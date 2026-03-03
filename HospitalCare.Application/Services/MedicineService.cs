using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IMedicineRepository _medicineRepository;

    public MedicineService(IMedicineRepository medicineRepository)
    {
        _medicineRepository = medicineRepository;
    }

    public async Task<IEnumerable<MedicineDto>> GetAllAsync()
    {
        var medicines = await _medicineRepository.GetAllAsync();
        return medicines.Select(MapToDto);
    }

    public async Task<IEnumerable<SearchMedicineDto>> SearchAsync(string searchTerm, int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Enumerable.Empty<SearchMedicineDto>();
        }

        var medicines = await _medicineRepository.SearchFuzzyAsync(searchTerm, limit);
        return medicines.Select(m => new SearchMedicineDto(
            m.Id,
            m.ProductName,
            m.SaltComposition,
            m.Manufacturer,
            m.SubCategory
        ));
    }

    public async Task<IEnumerable<string>> GetAllSubCategoriesAsync()
    {
        return await _medicineRepository.GetAllSubCategoriesAsync();
    }

    public async Task<IEnumerable<MedicineDto>> GetBySubCategoryAsync(string subCategory)
    {
        var medicines = await _medicineRepository.GetBySubCategoryAsync(subCategory);
        return medicines.Select(MapToDto);
    }

    private static MedicineDto MapToDto(Medicine medicine) => new(
        medicine.Id,
        medicine.SubCategory,
        medicine.ProductName,
        medicine.SaltComposition,
        medicine.Price,
        medicine.Manufacturer,
        medicine.Description,
        medicine.SideEffects,
        medicine.DrugInteractions,
        medicine.IsActive
    );
}
