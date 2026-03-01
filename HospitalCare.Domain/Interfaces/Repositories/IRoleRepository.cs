namespace HospitalCare.Domain.Interfaces.Repositories;

using HospitalCare.Domain.Entities;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name);
    Task<bool> NameExistsAsync(string name);
}
