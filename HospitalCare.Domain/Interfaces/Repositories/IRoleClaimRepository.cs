namespace HospitalCare.Domain.Interfaces.Repositories;

using HospitalCare.Domain.Entities;

public interface IRoleClaimRepository : IRepository<RoleClaim>
{
    Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(Guid roleId);
    Task<IEnumerable<Claim>> GetClaimsByRoleIdAsync(Guid roleId);
    Task<bool> ExistsAsync(Guid roleId, Guid claimId);
    Task DeleteByRoleIdAsync(Guid roleId);
    Task DeleteAsync(Guid roleId, Guid claimId);
}
