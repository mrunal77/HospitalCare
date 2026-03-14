namespace HospitalCare.Domain.Interfaces.Repositories;

using HospitalCare.Domain.Entities;

public interface IClaimRepository : IRepository<Claim>
{
    Task<Claim?> GetByNameAsync(string name);
    Task<IEnumerable<Claim>> GetActiveAsync();
    Task<IEnumerable<Claim>> GetByCategoryAsync(string category);
    Task<bool> NameExistsAsync(string name);
}
