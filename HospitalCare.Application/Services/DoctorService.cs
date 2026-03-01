using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IMapper _mapper;

    public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
    {
        _doctorRepository = doctorRepository;
        _mapper = mapper;
    }

    public async Task<DoctorDto?> GetByIdAsync(Guid id)
    {
        var doctor = await _doctorRepository.GetByIdAsync(id);
        return doctor is null ? null : _mapper.Map<DoctorDto>(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync()
    {
        var doctors = await _doctorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
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
        return _mapper.Map<DoctorDto>(created);
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
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }
}
