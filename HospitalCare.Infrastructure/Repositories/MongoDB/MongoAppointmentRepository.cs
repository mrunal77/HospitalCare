using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoAppointmentRepository : MongoRepository<Appointment>, IAppointmentRepository
{
    public MongoAppointmentRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
    {
        var filter = Builders<Appointment>.Filter.Eq(a => a.PatientId, patientId);
        var sort = Builders<Appointment>.Sort.Descending(a => a.AppointmentDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(Guid doctorId)
    {
        var filter = Builders<Appointment>.Filter.Eq(a => a.DoctorId, doctorId);
        var sort = Builders<Appointment>.Sort.Descending(a => a.AppointmentDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date)
    {
        var filter = Builders<Appointment>.Filter.Eq(a => a.AppointmentDate.Date, date.Date);
        var sort = Builders<Appointment>.Sort.Ascending(a => a.AppointmentDate);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }
}