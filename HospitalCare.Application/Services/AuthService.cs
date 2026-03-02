using System.Security.Cryptography;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using Polly;
using Polly.Retry;

namespace HospitalCare.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;
    private readonly ResiliencePipeline<AuthResponseDto?> _loginRetryPipeline;

    public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtService = jwtService;
        _loginRetryPipeline = BuildLoginRetryPipeline();
    }

    private static ResiliencePipeline<AuthResponseDto?> BuildLoginRetryPipeline()
    {
        return new ResiliencePipelineBuilder<AuthResponseDto?>()
            .AddRetry(new RetryStrategyOptions<AuthResponseDto?>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder<AuthResponseDto?>()
                    .Handle<MongoDB.Driver.MongoConnectionException>()
                    .Handle<MongoDB.Driver.MongoCommandException>()
                    .Handle<InvalidOperationException>(),
                OnRetry = args =>
                {
                    Console.WriteLine($"Login retry attempt {args.AttemptNumber + 1} due to: {args.Outcome.Exception?.Message}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task<(AuthResponseDto? Response, bool IsInactive)> LoginAsync(LoginDto dto)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            
            if (user is null)
                return (null, false);

            if (!user.IsActive)
                return (null, true);

            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return (null, false);

            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            return (GenerateAuthResponse(user, role?.Name ?? "Unknown"), false);
        }
        catch (MongoDB.Driver.MongoConnectionException)
        {
            throw;
        }
        catch (MongoDB.Driver.MongoCommandException)
        {
            throw;
        }
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return null;

        var role = await _roleRepository.GetByNameAsync(dto.Role);
        var roleId = role?.Id ?? (await _roleRepository.GetByNameAsync("HospitalEmployee"))?.Id 
            ?? throw new InvalidOperationException("Default role not found");

        var passwordHash = HashPassword(dto.Password);
        var user = new User(dto.Email, passwordHash, dto.FirstName, dto.LastName, roleId);

        var createdUser = await _userRepository.AddAsync(user);
        return GenerateAuthResponse(createdUser, role?.Name ?? "HospitalEmployee");
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            return false;

        user.UpdatePassword(HashPassword(dto.NewPassword));
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> UserExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        var roleDtos = new Dictionary<Guid, RoleDto>();
        
        var result = new List<UserDto>();
        foreach (var u in users)
        {
            if (!roleDtos.TryGetValue(u.RoleId, out var roleDto))
            {
                var role = await _roleRepository.GetByIdAsync(u.RoleId);
                roleDto = role is null ? null : new RoleDto(role.Id, role.Name, role.Description, role.Permission, role.IsActive, role.CreatedAt);
                if (roleDto is not null)
                    roleDtos[u.RoleId] = roleDto;
            }
            
            result.Add(new UserDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                roleDto?.Name ?? "Unknown",
                u.IsActive,
                u.CreatedAt
            ));
        }
        return result;
    }

    public async Task<bool> ResetUserPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        user.UpdatePassword(HashPassword(newPassword));
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
            return null;

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        return new UserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            role?.Name ?? "Unknown",
            user.IsActive,
            user.CreatedAt
        );
    }

    public async Task<bool> EnableUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        user.Activate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DisableUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        user.Deactivate();
        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
            return false;

        await _userRepository.DeleteAsync(userId);
        return true;
    }

    private AuthResponseDto GenerateAuthResponse(User user, string roleName)
    {
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName, roleName);
        return new AuthResponseDto(
            token,
            user.Email,
            user.FirstName,
            user.LastName,
            roleName,
            _jwtService.GetTokenExpiration()
        );
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var hashBytes = Convert.FromBase64String(storedHash);
        var salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);

        for (int i = 0; i < 32; i++)
        {
            if (hashBytes[i + 16] != hash[i])
                return false;
        }
        return true;
    }
}
