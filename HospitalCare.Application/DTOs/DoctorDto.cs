namespace HospitalCare.Application.DTOs;

public record DoctorDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Specialization,
    string Email,
    string Phone,
    string LicenseNumber
);

public record CreateDoctorDto(
    string FirstName,
    string LastName,
    string Specialization,
    string Email,
    string Phone,
    string LicenseNumber
);
