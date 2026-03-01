using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoDoctorRepository : MongoRepository<Doctor>, IDoctorRepository
{
    public MongoDoctorRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Doctor>> GetBySpecializationAsync(string specialization)
    {
        var filter = Builders<Doctor>.Filter.Regex(d => d.Specialization, 
            new BsonRegularExpression(specialization, "i"));
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber)
    {
        var filter = Builders<Doctor>.Filter.Eq(d => d.LicenseNumber, licenseNumber);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }
}
