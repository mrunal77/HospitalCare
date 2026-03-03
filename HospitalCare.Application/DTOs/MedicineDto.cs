using HospitalCare.Domain.Entities;

namespace HospitalCare.Application.DTOs;

public record MedicineDto(
    Guid Id,
    string SubCategory,
    string ProductName,
    string SaltComposition,
    decimal? Price,
    string Manufacturer,
    string Description,
    string? SideEffects,
    string? DrugInteractions,
    bool IsActive
);

public record SearchMedicineDto(
    Guid Id,
    string ProductName,
    string SaltComposition,
    string Manufacturer,
    string SubCategory
);
