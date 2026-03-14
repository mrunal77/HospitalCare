using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IRoleClaimService
{
    Task<IEnumerable<RoleClaimDto>> GetByRoleIdAsync(Guid roleId);
    Task<RoleClaimDto> CreateAsync(CreateRoleClaimDto dto);
    Task DeleteAsync(Guid roleId, Guid claimId);
    Task UpdateRoleClaimsAsync(UpdateRoleClaimsDto dto);
    Task<IEnumerable<ClaimDto>> GetClaimsByRoleIdAsync(Guid roleId);
}
