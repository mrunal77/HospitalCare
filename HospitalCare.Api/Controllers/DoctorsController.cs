using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorService _doctorService;

    public DoctorsController(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetAll()
    {
        var doctors = await _doctorService.GetAllAsync();
        return Ok(doctors);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<DoctorDto>> GetById(Guid id)
    {
        var doctor = await _doctorService.GetByIdAsync(id);
        return doctor is null ? NotFound() : Ok(doctor);
    }

    [HttpGet("specialization/{specialization}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<DoctorDto>>> GetBySpecialization(string specialization)
    {
        var doctors = await _doctorService.GetBySpecializationAsync(specialization);
        return Ok(doctors);
    }

    [HttpPost]
    [Authorize(Policy = "FullAccess")]
    public async Task<ActionResult<DoctorDto>> Create([FromBody] CreateDoctorDto dto)
    {
        var doctor = await _doctorService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "FullAccess")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _doctorService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
