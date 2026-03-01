using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Interfaces.Repositories;

public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByEmailAsync(string email);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
}
