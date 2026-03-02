using System.Security.Claims;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var (result, isInactive) = await _authService.LoginAsync(dto);
        
        if (isInactive)
            return Unauthorized(new { message = "Your account has been deactivated. Please contact Administrator." });

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin,HospitalEmployee")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterUserDto dto)
    {
        if (await _authService.UserExistsAsync(dto.Email))
            return BadRequest(new { message = "User with this email already exists" });

        var result = await _authService.RegisterAsync(dto);
        if (result is null)
            return BadRequest(new { message = "Failed to create user" });

        return CreatedAtAction(nameof(Login), result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = Guid.Parse(userIdClaim);
        var result = await _authService.ChangePasswordAsync(userId, dto);

        if (!result)
            return BadRequest(new { message = "Failed to change password" });

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult GetCurrentUser()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role
        });
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPost("reset-password/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ResetUserPassword(Guid userId, [FromBody] ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(new { message = "New password is required" });

        var result = await _authService.ResetUserPasswordAsync(userId, dto.NewPassword);

        if (!result)
            return BadRequest(new { message = "Failed to reset password" });

        return Ok(new { message = "Password reset successfully" });
    }
}
