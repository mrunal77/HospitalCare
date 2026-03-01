namespace HospitalCare.Domain.Interfaces.Repositories;

using HospitalCare.Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetByRoleAsync(Guid roleId);
    Task<bool> EmailExistsAsync(string email);
}
