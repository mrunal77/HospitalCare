namespace HospitalCare.Application.DTOs;

public record ClaimDto(
    Guid Id,
    string Name,
    string Description,
    string Category,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateClaimDto(
    string Name,
    string Description,
    string Category
);

public record UpdateClaimDto(
    string Name,
    string Description,
    string Category
);
