using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IDoctorService
{
    Task<DoctorDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<DoctorDto>> GetAllAsync();
    Task<DoctorDto> CreateAsync(CreateDoctorDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<DoctorDto>> GetBySpecializationAsync(string specialization);
}
