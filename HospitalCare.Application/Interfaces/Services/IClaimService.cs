using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IClaimService
{
    Task<IEnumerable<ClaimDto>> GetAllAsync();
    Task<ClaimDto?> GetByIdAsync(Guid id);
    Task<ClaimDto> CreateAsync(CreateClaimDto dto);
    Task<ClaimDto> UpdateAsync(Guid id, UpdateClaimDto dto);
    Task DeleteAsync(Guid id);
    Task<ClaimDto> ActivateAsync(Guid id);
    Task<ClaimDto> DeactivateAsync(Guid id);
    Task<IEnumerable<ClaimDto>> GetActiveAsync();
    Task<IEnumerable<ClaimDto>> GetByCategoryAsync(string category);
}
