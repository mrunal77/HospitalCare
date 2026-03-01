using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IAppointmentService
{
    Task<AppointmentDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<bool> CancelAsync(Guid id, string? reason = null);
    Task<bool> CompleteAsync(Guid id, string? notes = null);
    Task<AppointmentDto?> RescheduleAsync(Guid id, RescheduleAppointmentDto dto);
    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(Guid doctorId);
}
