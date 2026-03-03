using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;

namespace HospitalCare.Application.Services;

public class PrescriptionService : IPrescriptionService
{
    private readonly IPrescriptionRepository _prescriptionRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;

    public PrescriptionService(
        IPrescriptionRepository prescriptionRepository,
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository)
    {
        _prescriptionRepository = prescriptionRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<PrescriptionDto?> GetByIdAsync(Guid id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        return prescription is null ? null : await MapToDtoAsync(prescription);
    }

    public async Task<IEnumerable<PrescriptionDto>> GetAllAsync()
    {
        var prescriptions = await _prescriptionRepository.GetAllAsync();
        var dtos = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            dtos.Add(await MapToDtoAsync(prescription));
        }
        return dtos;
    }

    public async Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto)
    {
        var medicines = dto.Medicines.Select(m => new MedicineDetail(
            m.Name,
            m.Dosage,
            m.Amount,
            m.Routine,
            m.Instructions,
            m.DurationDays
        )).ToList();

        var prescription = new Prescription(
            dto.AppointmentId,
            dto.DoctorId,
            dto.PatientId,
            dto.Diagnosis,
            medicines,
            dto.Notes
        );

        var created = await _prescriptionRepository.AddAsync(prescription);
        return await MapToDtoAsync(created);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription is null) return false;

        await _prescriptionRepository.DeleteAsync(id);
        return true;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetByPatientIdAsync(Guid patientId)
    {
        var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId);
        var dtos = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            dtos.Add(await MapToDtoAsync(prescription));
        }
        return dtos;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetByDoctorIdAsync(Guid doctorId)
    {
        var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctorId);
        var dtos = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            dtos.Add(await MapToDtoAsync(prescription));
        }
        return dtos;
    }

    public async Task<IEnumerable<PrescriptionDto>> GetByAppointmentIdAsync(Guid appointmentId)
    {
        var prescriptions = await _prescriptionRepository.GetByAppointmentIdAsync(appointmentId);
        var dtos = new List<PrescriptionDto>();
        foreach (var prescription in prescriptions)
        {
            dtos.Add(await MapToDtoAsync(prescription));
        }
        return dtos;
    }

    public async Task<PrescriptionDto?> AddMedicineAsync(Guid prescriptionId, AddMedicineDto dto)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
        if (prescription is null) return null;

        var medicine = new MedicineDetail(
            dto.Name,
            dto.Dosage,
            dto.Amount,
            dto.Routine,
            dto.Instructions,
            dto.DurationDays
        );

        prescription.AddMedicine(medicine);
        await _prescriptionRepository.UpdateAsync(prescription);
        return await MapToDtoAsync(prescription);
    }

    public async Task<bool> RemoveMedicineAsync(Guid prescriptionId, Guid medicineId)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
        if (prescription is null) return false;

        prescription.RemoveMedicine(medicineId);
        await _prescriptionRepository.UpdateAsync(prescription);
        return true;
    }

    public async Task<PrescriptionDto?> UpdateAsync(Guid id, UpdatePrescriptionDto dto)
    {
        var prescription = await _prescriptionRepository.GetByIdAsync(id);
        if (prescription is null) return null;

        if (!string.IsNullOrEmpty(dto.Diagnosis))
        {
            prescription.UpdateDiagnosis(dto.Diagnosis);
        }

        if (dto.Notes != null)
        {
            prescription.UpdateNotes(dto.Notes);
        }

        await _prescriptionRepository.UpdateAsync(prescription);
        return await MapToDtoAsync(prescription);
    }

    private async Task<PrescriptionDto> MapToDtoAsync(Prescription prescription)
    {
        var patient = await _patientRepository.GetByIdAsync(prescription.PatientId);
        var doctor = await _doctorRepository.GetByIdAsync(prescription.DoctorId);

        return new PrescriptionDto(
            prescription.Id,
            prescription.AppointmentId,
            prescription.DoctorId,
            prescription.PatientId,
            doctor is null ? "Unknown" : $"{doctor.FirstName} {doctor.LastName}",
            patient is null ? "Unknown" : $"{patient.FirstName} {patient.LastName}",
            prescription.Diagnosis,
            prescription.Medicines.Select(m => new MedicineDetailDto(
                m.Id,
                m.Name,
                m.Dosage,
                m.Amount,
                m.Routine,
                m.Instructions,
                m.DurationDays
            )).ToList(),
            prescription.Notes,
            prescription.PrescriptionDate
        );
    }
}
