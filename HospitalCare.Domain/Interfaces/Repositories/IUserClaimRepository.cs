namespace HospitalCare.Domain.Interfaces.Repositories;

using HospitalCare.Domain.Entities;

public interface IUserClaimRepository : IRepository<UserClaim>
{
    Task<IEnumerable<UserClaim>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Claim>> GetClaimsByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, Guid claimId);
    Task DeleteByUserIdAsync(Guid userId);
    Task DeleteAsync(Guid userId, Guid claimId);
}
