using System.Security.Cryptography;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user is null || !user.IsActive)
            return null;

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            return null;

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterUserDto dto)
    {
        if (await _userRepository.EmailExistsAsync(dto.Email))
            return null;

        if (!Enum.TryParse<UserRole>(dto.Role, out var role))
            role = UserRole.HospitalEmployee;

        var passwordHash = HashPassword(dto.Password);
        var user = new User(dto.Email, passwordHash, dto.FirstName, dto.LastName, role);

        var createdUser = await _userRepository.AddAsync(user);
        return GenerateAuthResponse(createdUser);
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
        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.Role.ToString(),
            u.IsActive,
            u.CreatedAt
        ));
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

    private AuthResponseDto GenerateAuthResponse(User user)
    {
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName, user.Role.ToString());
        return new AuthResponseDto(
            token,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
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
