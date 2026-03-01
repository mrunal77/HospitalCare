using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Interfaces.Repositories;

public interface IAppointmentRepository : IRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId);
    Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date);
}
