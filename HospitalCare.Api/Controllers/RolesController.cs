using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "FullAccess")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
    {
        var roles = await _roleService.GetAllAsync();
        return Ok(roles);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetActiveRoles()
    {
        var roles = await _roleService.GetAllAsync();
        var activeRoles = roles.Where(r => r.IsActive);
        return Ok(activeRoles);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<RoleDto>> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        if (role is null)
        {
            return NotFound(new { message = $"Role with id '{id}' not found" });
        }
        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto)
    {
        try
        {
            var role = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoleDto>> Update(Guid id, [FromBody] UpdateRoleDto dto)
    {
        try
        {
            var role = await _roleService.UpdateAsync(id, dto);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<RoleDto>> Activate(Guid id)
    {
        try
        {
            var role = await _roleService.ActivateAsync(id);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<RoleDto>> Deactivate(Guid id)
    {
        try
        {
            var role = await _roleService.DeactivateAsync(id);
            return Ok(role);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
