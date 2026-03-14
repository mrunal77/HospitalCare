using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "FullAccess")]
public class UserClaimsController : ControllerBase
{
    private readonly IUserClaimService _userClaimService;

    public UserClaimsController(IUserClaimService userClaimService)
    {
        _userClaimService = userClaimService;
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<IEnumerable<UserClaimDto>>> GetByUserId(Guid userId)
    {
        var userClaims = await _userClaimService.GetByUserIdAsync(userId);
        return Ok(userClaims);
    }

    [HttpGet("user/{userId:guid}/effective")]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetEffectiveClaims(Guid userId)
    {
        try
        {
            var claims = await _userClaimService.GetEffectiveClaimsAsync(userId);
            return Ok(claims);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<UserClaimDto>> Create([FromBody] CreateUserClaimDto dto)
    {
        try
        {
            var userClaim = await _userClaimService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByUserId), new { userId = userClaim.UserId }, userClaim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult> Update([FromBody] UpdateUserClaimsDto dto)
    {
        try
        {
            await _userClaimService.UpdateUserClaimsAsync(dto);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{userId:guid}/{claimId:guid}")]
    public async Task<ActionResult> Delete(Guid userId, Guid claimId)
    {
        try
        {
            await _userClaimService.DeleteAsync(userId, claimId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
