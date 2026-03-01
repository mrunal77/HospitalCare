namespace HospitalCare.Application.DTOs;

public record RoleDto(
    Guid Id,
    string Name,
    string Description,
    string Permission,
    bool IsActive,
    DateTime CreatedAt
);

public record CreateRoleDto(
    string Name,
    string Description,
    string Permission
);

public record UpdateRoleDto(
    string Name,
    string Description,
    string Permission
);
