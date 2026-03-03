using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Interfaces.Services;

public interface IPrescriptionService
{
    Task<PrescriptionDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<PrescriptionDto>> GetAllAsync();
    Task<PrescriptionDto> CreateAsync(CreatePrescriptionDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<PrescriptionDto>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<PrescriptionDto>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<PrescriptionDto>> GetByAppointmentIdAsync(Guid appointmentId);
    Task<PrescriptionDto?> AddMedicineAsync(Guid prescriptionId, AddMedicineDto dto);
    Task<bool> RemoveMedicineAsync(Guid prescriptionId, Guid medicineId);
    Task<PrescriptionDto?> UpdateAsync(Guid id, UpdatePrescriptionDto dto);
}
