using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "FullAccess")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var users = await _authService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _authService.GetUserByIdAsync(id);
        if (user is null)
            return NotFound(new { message = "User not found" });
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponseDto>> Create([FromBody] RegisterUserDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result is null)
            return BadRequest(new { message = "User already exists or invalid role" });
        return CreatedAtAction(nameof(GetById), new { id = result.Email }, result);
    }

    [HttpPut("{id:guid}/enable")]
    public async Task<IActionResult> EnableUser(Guid id)
    {
        var result = await _authService.EnableUserAsync(id);
        if (!result)
            return NotFound(new { message = "User not found" });
        return Ok(new { message = "User enabled successfully" });
    }

    [HttpPut("{id:guid}/disable")]
    public async Task<IActionResult> DisableUser(Guid id)
    {
        var result = await _authService.DisableUserAsync(id);
        if (!result)
            return NotFound(new { message = "User not found" });
        return Ok(new { message = "User disabled successfully" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var result = await _authService.DeleteUserAsync(id);
        if (!result)
            return NotFound(new { message = "User not found" });
        return NoContent();
    }

    [HttpPut("{id:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] string newPassword)
    {
        var result = await _authService.ResetUserPasswordAsync(id, newPassword);
        if (!result)
            return NotFound(new { message = "User not found" });
        return Ok(new { message = "Password reset successfully" });
    }
}
