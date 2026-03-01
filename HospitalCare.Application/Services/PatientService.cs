using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> GetByIdAsync(Guid id)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        return patient is null ? null : MapToDto(patient);
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        var patients = await _patientRepository.GetAllAsync();
        return patients.Select(MapToDto);
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
        return MapToDto(created);
    }

    public async Task<PatientDto?> UpdateAsync(Guid id, UpdatePatientDto dto)
    {
        var patient = await _patientRepository.GetByIdAsync(id);
        if (patient is null) return null;

        patient.UpdateContactInfo(dto.Email, dto.Phone, dto.Address);
        await _patientRepository.UpdateAsync(patient);
        return MapToDto(patient);
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
        return patients.Select(MapToDto);
    }

    private static PatientDto MapToDto(Patient patient) => new(
        patient.Id,
        patient.FirstName,
        patient.LastName,
        patient.DateOfBirth,
        patient.Email,
        patient.Phone,
        patient.Address
    );
}
