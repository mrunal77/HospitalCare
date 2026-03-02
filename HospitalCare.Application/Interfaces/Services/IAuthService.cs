using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    Task<AuthResponseDto?> RegisterAsync(RegisterUserDto dto);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<bool> UserExistsAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<bool> ResetUserPasswordAsync(Guid userId, string newPassword);
    Task<bool> EnableUserAsync(Guid userId);
    Task<bool> DisableUserAsync(Guid userId);
    Task<bool> DeleteUserAsync(Guid userId);
}
