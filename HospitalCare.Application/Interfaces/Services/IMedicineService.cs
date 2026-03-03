using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IMedicineService
{
    Task<IEnumerable<MedicineDto>> GetAllAsync();
    Task<IEnumerable<SearchMedicineDto>> SearchAsync(string searchTerm, int limit = 20);
    Task<IEnumerable<string>> GetAllSubCategoriesAsync();
    Task<IEnumerable<MedicineDto>> GetBySubCategoryAsync(string subCategory);
}
