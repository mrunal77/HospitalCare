using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<DoctorDto?> GetByIdAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        return doctor is null ? null : MapToDto(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return doctors.Select(MapToDto);
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorDto dto)
    {
        var doctor = new Doctor(
            dto.FirstName,
            dto.LastName,
            dto.Specialization,
            dto.Email,
            dto.Phone,
            dto.LicenseNumber
        );

        var created = await _doctorRepository.AddAsync(doctor);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        if (doctor is null) return false;

        await _doctorRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<DoctorDto>> GetBySpecializationAsync(string specialization)
    {
        var doctors = await _doctorRepository.GetBySpecializationAsync(specialization);
        return doctors.Select(MapToDto);
    }

    private static DoctorDto MapToDto(Doctor doctor) => new(
        doctor.Id,
        doctor.FirstName,
        doctor.LastName,
        doctor.Specialization,
        doctor.Email,
        doctor.Phone,
        doctor.LicenseNumber
    );
}
