using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Interfaces.Repositories;

public interface IDoctorRepository : IRepository<Doctor>
{
    Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization);
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber);
}
