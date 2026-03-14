using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "FullAccess")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetAll()
    {
        var claims = await _claimService.GetAllAsync();
        return Ok(claims);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetActive()
    {
        var claims = await _claimService.GetActiveAsync();
        return Ok(claims);
    }

    [HttpGet("category/{category}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ClaimDto>>> GetByCategory(string category)
    {
        var claims = await _claimService.GetByCategoryAsync(category);
        return Ok(claims);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ClaimDto>> GetById(Guid id)
    {
        var claim = await _claimService.GetByIdAsync(id);
        if (claim is null)
        {
            return NotFound(new { message = $"Claim with id '{id}' not found" });
        }
        return Ok(claim);
    }

    [HttpPost]
    public async Task<ActionResult<ClaimDto>> Create([FromBody] CreateClaimDto dto)
    {
        try
        {
            var claim = await _claimService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = claim.Id }, claim);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClaimDto>> Update(Guid id, [FromBody] UpdateClaimDto dto)
    {
        try
        {
            var claim = await _claimService.UpdateAsync(id, dto);
            return Ok(claim);
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
            await _claimService.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/activate")]
    public async Task<ActionResult<ClaimDto>> Activate(Guid id)
    {
        try
        {
            var claim = await _claimService.ActivateAsync(id);
            return Ok(claim);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<ActionResult<ClaimDto>> Deactivate(Guid id)
    {
        try
        {
            var claim = await _claimService.DeactivateAsync(id);
            return Ok(claim);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
