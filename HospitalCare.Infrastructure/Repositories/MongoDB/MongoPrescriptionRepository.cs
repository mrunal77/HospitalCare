using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoPrescriptionRepository : MongoRepository<Prescription>, IPrescriptionRepository
{
    public MongoPrescriptionRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(Guid patientId)
    {
        var filter = Builders<Prescription>.Filter.Eq(p => p.PatientId, patientId);
        var sort = Builders<Prescription>.Sort.Descending(p => p.PrescriptionDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(Guid doctorId)
    {
        var filter = Builders<Prescription>.Filter.Eq(p => p.DoctorId, doctorId);
        var sort = Builders<Prescription>.Sort.Descending(p => p.PrescriptionDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> GetByAppointmentIdAsync(Guid appointmentId)
    {
        var filter = Builders<Prescription>.Filter.Eq(p => p.AppointmentId, appointmentId);
        var sort = Builders<Prescription>.Sort.Descending(p => p.PrescriptionDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }
}
