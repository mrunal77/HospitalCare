namespace HospitalCare.Application.DTOs;

public record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Email,
    string Phone,
    string? Address
);

public record CreatePatientDto(
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Email,
    string Phone,
    string? Address
);

public record UpdatePatientDto(
    string Email,
    string Phone,
    string? Address
);
