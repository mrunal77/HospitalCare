using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "FullAccess")]
public class RoleClaimsController : ControllerBase
{
    private readonly IRoleClaimService _roleClaimService;

    public RoleClaimsController(IRoleClaimService roleClaimService)
    {
        _roleClaimService = roleClaimService;
    }

    [HttpGet("role/{roleId:guid}")]
    public async Task<ActionResult<IEnumerable<RoleClaimDto>>> GetByRoleId(Guid roleId)
    {
        var roleClaims = await _roleClaimService.GetByRoleIdAsync(roleId);
        return Ok(roleClaims);
    }

    [HttpGet("role/{roleId:guid}/claims")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetClaimsByRoleId(Guid roleId)
    {
        var claims = await _roleClaimService.GetClaimsByRoleIdAsync(roleId);
        return Ok(claims);
    }

    [HttpPost]
    public async Task<ActionResult<RoleClaimDto>> Create([FromBody] CreateRoleClaimDto dto)
    {
        try
        {
            var roleClaim = await _roleClaimService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByRoleId), new { roleId = roleClaim.RoleId }, roleClaim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateRoleClaimsDto dto)
    {
        try
        {
            await _roleClaimService.UpdateRoleClaimsAsync(dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{roleId:guid}/{claimId:guid}")]
    public async Task<ActionResult> Delete(Guid roleId, Guid claimId)
    {
        try
        {
            await _roleClaimService.DeleteAsync(roleId, claimId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
