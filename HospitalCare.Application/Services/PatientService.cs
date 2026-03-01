using AutoMapper;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient is null ? null : _mapper.Map<PatientDto>(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto dto)
    {
        var patient = new Patient(
            dto.FirstName,
            dto.LastName,
            dto.DateOfBirth,
            dto.Email,
            dto.Phone,
            dto.Address
        );

        var created = await _patientRepository.AddAsync(patient);
        return _mapper.Map<PatientDto>(created);
    }

    public async Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null) return null;

        patient.UpdateContactInfo(dto.Email, dto.Phone, dto.Address);
        await _patientRepository.UpdateAsync(patient);
        return _mapper.Map<PatientDto>(patient);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null) return false;

        await _patientRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<PatientDto>> SearchByNameAsync(string name)
    {
        var patients = await _patientRepository.SearchByNameAsync(name);
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}
