namespace HospitalCare.Application.DTOs;

public record AppointmentDto(
    Guid Id,
    Guid PatientId,
    Guid DoctorId,
    string PatientName,
    string DoctorName,
    DateTime AppointmentDate,
    int DurationMinutes,
    string Reason,
    string? Notes,
    string Status
);

public record CreateAppointmentDto(
    Guid PatientId,
    Guid DoctorId,
    DateTime AppointmentDate,
    int DurationMinutes,
    string Reason,
    string? Notes
);

public record RescheduleAppointmentDto(
    DateTime NewDate,
    int NewDurationMinutes
);
