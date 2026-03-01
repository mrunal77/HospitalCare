using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByPatientId(Guid patientId)
    {
        var appointments = await _appointmentService.GetByPatientIdAsync(patientId);
        return Ok(appointments);
    }

    [HttpGet("doctor/{doctorId:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetByDoctorId(Guid doctorId)
    {
        var appointments = await _appointmentService.GetByDoctorIdAsync(doctorId);
        return Ok(appointments);
    }

    [HttpPost]
    [Authorize(Policy = "EmployeeAccess")]
    public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto dto)
    {
        var appointment = await _appointmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id:guid}/cancel")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult> Cancel(Guid id, [FromQuery] string? reason = null)
    {
        var result = await _appointmentService.CancelAsync(id, reason);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/complete")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult> Complete(Guid id, [FromQuery] string? notes = null)
    {
        var result = await _appointmentService.CompleteAsync(id, notes);
        return result ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/reschedule")]
    [Authorize(Policy = "EmployeeAccess")]
    public async Task<ActionResult<AppointmentDto>> Reschedule(Guid id, [FromBody] RescheduleAppointmentDto dto)
    {
        var appointment = await _appointmentService.RescheduleAsync(id, dto);
        return appointment is null ? NotFound() : Ok(appointment);
    }
}
