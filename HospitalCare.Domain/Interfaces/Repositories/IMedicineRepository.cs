using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Interfaces.Repositories;

public interface IMedicineRepository : IRepository<Medicine>
{
    Task<IEnumerable<Medicine>> GetBySubCategoryAsync(string subCategory);
    Task<IEnumerable<Medicine>> SearchByNameAsync(string searchTerm);
    Task<IEnumerable<Medicine>> SearchFuzzyAsync(string searchTerm, int limit = 20);
    Task<IEnumerable<string>> GetAllSubCategoriesAsync();
    Task BulkInsertAsync(IEnumerable<Medicine> medicines);
}
