using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        return appointment is null ? null : await MapToDtoAsync(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        var dtos = new List<AppointmentDto>();
        foreach (var appointment in appointments)
        {
            dtos.Add(await MapToDtoAsync(appointment));
        }
        return dtos;
    }

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        var appointment = new Appointment(
            dto.PatientId,
            dto.DoctorId,
            dto.AppointmentDate,
            TimeSpan.FromMinutes(dto.DurationMinutes),
            dto.Reason,
            dto.Notes
        );

        var created = await _appointmentRepository.AddAsync(appointment);
        return await MapToDtoAsync(created);
    }

    public async Task<bool> CancelAsync(Guid id, string? reason = null)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return false;

        appointment.Cancel(reason);
        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }

    public async Task<bool> CompleteAsync(Guid id, string? notes = null)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return false;

        appointment.Complete(notes);
        await _appointmentRepository.UpdateAsync(appointment);
        return true;
    }

    public async Task<AppointmentDto?> RescheduleAsync(Guid id, RescheduleAppointmentDto dto)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment is null) return null;

        appointment.Reschedule(dto.NewDate, TimeSpan.FromMinutes(dto.NewDurationMinutes));
        await _appointmentRepository.UpdateAsync(appointment);
        return await MapToDtoAsync(appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId)
    {
        var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
        var dtos = new List<AppointmentDto>();
        foreach (var appointment in appointments)
        {
            dtos.Add(await MapToDtoAsync(appointment));
        }
        return dtos;
    }

    public async Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(Guid doctorId)
    {
        var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
        var dtos = new List<AppointmentDto>();
        foreach (var appointment in appointments)
        {
            dtos.Add(await MapToDtoAsync(appointment));
        }
        return dtos;
    }

    private async Task<AppointmentDto> MapToDtoAsync(Appointment appointment)
    {
        var patient = await _patientRepository.GetByIdAsync(appointment.PatientId);
        var doctor = await _doctorRepository.GetByIdAsync(appointment.DoctorId);

        return new AppointmentDto(
            appointment.Id,
            appointment.PatientId,
            appointment.DoctorId,
            patient is null ? "Unknown" : $"{patient.FirstName} {patient.LastName}",
            doctor is null ? "Unknown" : $"{doctor.FirstName} {doctor.LastName}",
            appointment.AppointmentDate,
            (int)appointment.Duration.TotalMinutes,
            appointment.Reason,
            appointment.Notes,
            appointment.Status.ToString()
        );
    }
}
