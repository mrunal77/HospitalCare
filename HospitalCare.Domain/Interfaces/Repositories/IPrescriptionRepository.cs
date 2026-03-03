using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Interfaces.Repositories;

public interface IPrescriptionRepository : IRepository<Prescription>
{
    Task<IEnumerable<Prescription>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Prescription>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Prescription>> GetByAppointmentIdAsync(Guid appointmentId);
}
