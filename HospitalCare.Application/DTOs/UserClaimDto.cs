namespace HospitalCare.Application.DTOs;

public record UserClaimDto(
    Guid Id,
    Guid UserId,
    Guid ClaimId,
    string? ClaimName,
    string? ClaimDescription,
    string? ClaimCategory,
    DateTime CreatedAt
);

public record CreateUserClaimDto(
    Guid UserId,
    Guid ClaimId
);

public record UpdateUserClaimsDto(
    Guid UserId,
    List<Guid> ClaimIds
);
