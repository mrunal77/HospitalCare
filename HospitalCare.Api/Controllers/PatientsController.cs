using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
    {
        var patients = await _patientService.GetAllAsync();
        return Ok(patients);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<PatientDto>> GetById(Guid id)
    {
        var patient = await _patientService.GetByIdAsync(id);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpGet("search")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<PatientDto>>> Search([FromQuery] string name)
    {
        var patients = await _patientService.SearchByNameAsync(name);
        return Ok(patients);
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeAccess")]
    public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto dto)
    {
        var patient = await _patientService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "EmployeeAccess")]
    public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto dto)
    {
        var patient = await _patientService.UpdateAsync(id, dto);
        return patient is null ? NotFound() : Ok(patient);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "FullAccess")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _patientService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}
