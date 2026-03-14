namespace HospitalCare.Application.DTOs;

public record RoleClaimDto(
    Guid Id,
    Guid RoleId,
    Guid ClaimId,
    string? ClaimName,
    string? ClaimDescription,
    string? ClaimCategory,
    DateTime CreatedAt
);

public record CreateRoleClaimDto(
    Guid RoleId,
    Guid ClaimId
);

public record UpdateRoleClaimsDto(
    Guid RoleId,
    List<Guid> ClaimIds
);
