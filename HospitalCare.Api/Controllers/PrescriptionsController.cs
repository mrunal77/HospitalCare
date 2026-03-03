using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PrescriptionsController : ControllerBase
{
    private readonly IPrescriptionService _prescriptionService;

    public PrescriptionsController(IPrescriptionService prescriptionService)
    {
        _prescriptionService = prescriptionService;
    }

    [HttpGet]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetAll()
    {
        var prescriptions = await _prescriptionService.GetAllAsync();
        return Ok(prescriptions);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<PrescriptionDto>> GetById(Guid id)
    {
        var prescription = await _prescriptionService.GetByIdAsync(id);
        return prescription is null ? NotFound() : Ok(prescription);
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Policy = "PatientAccess")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetByPatientId(Guid patientId)
    {
        var prescriptions = await _prescriptionService.GetByPatientIdAsync(patientId);
        return Ok(prescriptions);
    }

    [HttpGet("doctor/{doctorId:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetByDoctorId(Guid doctorId)
    {
        var prescriptions = await _prescriptionService.GetByDoctorIdAsync(doctorId);
        return Ok(prescriptions);
    }

    [HttpGet("appointment/{appointmentId:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<IEnumerable<PrescriptionDto>>> GetByAppointmentId(Guid appointmentId)
    {
        var prescriptions = await _prescriptionService.GetByAppointmentIdAsync(appointmentId);
        return Ok(prescriptions);
    }

    [HttpPost]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<PrescriptionDto>> Create([FromBody] CreatePrescriptionDto dto)
    {
        var prescription = await _prescriptionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = prescription.Id }, prescription);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<PrescriptionDto>> Update(Guid id, [FromBody] UpdatePrescriptionDto dto)
    {
        var prescription = await _prescriptionService.UpdateAsync(id, dto);
        return prescription is null ? NotFound() : Ok(prescription);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _prescriptionService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/medicines")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult<PrescriptionDto>> AddMedicine(Guid id, [FromBody] AddMedicineDto dto)
    {
        var prescription = await _prescriptionService.AddMedicineAsync(id, dto);
        return prescription is null ? NotFound() : Ok(prescription);
    }

    [HttpDelete("{id:guid}/medicines/{medicineId:guid}")]
    [Authorize(Policy = "DoctorAccess")]
    public async Task<ActionResult> RemoveMedicine(Guid id, Guid medicineId)
    {
        var result = await _prescriptionService.RemoveMedicineAsync(id, medicineId);
        return result ? NoContent() : NotFound();
    }
}
