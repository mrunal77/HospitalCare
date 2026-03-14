using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IUserClaimService
{
    Task<IEnumerable<UserClaimDto>> GetByUserIdAsync(Guid userId);
    Task<UserClaimDto> CreateAsync(CreateUserClaimDto dto);
    Task DeleteAsync(Guid userId, Guid claimId);
    Task UpdateUserClaimsAsync(UpdateUserClaimsDto dto);
    Task<IEnumerable<ClaimDto>> GetEffectiveClaimsAsync(Guid userId);
}
