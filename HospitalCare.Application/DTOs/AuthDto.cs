namespace HospitalCare.Application.DTOs;

public record AuthResponseDto(
    string Token,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    DateTime Expiration
);

public record LoginDto(
    string Email,
    string Password
);

public record RegisterUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

public record ResetPasswordDto(
    string NewPassword
);

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);
