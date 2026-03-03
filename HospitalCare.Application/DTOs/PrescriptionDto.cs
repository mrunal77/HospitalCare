using HospitalCare.Domain.Entities;

namespace HospitalCare.Application.DTOs;

public record PrescriptionDto(
    Guid Id,
    Guid AppointmentId,
    Guid DoctorId,
    Guid PatientId,
    string DoctorName,
    string PatientName,
    string Diagnosis,
    List<MedicineDetailDto> Medicines,
    string? Notes,
    DateTime PrescriptionDate
);

public record MedicineDetailDto(
    Guid Id,
    string Name,
    string Dosage,
    string Amount,
    string Routine,
    string? Instructions,
    int DurationDays
);

public record CreatePrescriptionDto(
    Guid AppointmentId,
    Guid DoctorId,
    Guid PatientId,
    string Diagnosis,
    List<CreateMedicineDetailDto> Medicines,
    string? Notes
);

public record CreateMedicineDetailDto(
    string Name,
    string Dosage,
    string Amount,
    string Routine,
    string? Instructions,
    int DurationDays
);

public record UpdatePrescriptionDto(
    string? Diagnosis,
    string? Notes
);

public record AddMedicineDto(
    string Name,
    string Dosage,
    string Amount,
    string Routine,
    string? Instructions,
    int DurationDays
);
