using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IPatientService
{
    Task<PatientDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PatientDto>> GetAllAsync();
    Task<PatientDto> CreateAsync(CreatePatientDto dto);
    Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PatientDto>> SearchByNameAsync(string name);
}
