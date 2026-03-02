using HospitalCare.Infrastructure.Data.MongoDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecializationsController : ControllerBase
{
    private readonly MongoDbContext _context;

    public SpecializationsController(MongoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll()
    {
        var specializations = await _context.Specializations
            .Find(_ => true)
            .ToListAsync();
        return Ok(specializations);
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<ActionResult> GetActive()
    {
        var specializations = await _context.Specializations
            .Find(s => s.IsActive)
            .ToListAsync();
        return Ok(specializations);
    }
}
