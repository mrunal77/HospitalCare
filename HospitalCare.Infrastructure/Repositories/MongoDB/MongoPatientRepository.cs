using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoPatientRepository : MongoRepository<Patient>, IPatientRepository
{
    public MongoPatientRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<Patient?> GetByEmailAsync(string email)
    {
        var filter = Builders<Patient>.Filter.Eq(p => p.Email, email);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        var filter = Builders<Patient>.Filter.Or(
            Builders<Patient>.Filter.Regex(p => p.FirstName, new BsonRegularExpression(name, "i")),
            Builders<Patient>.Filter.Regex(p => p.LastName, new BsonRegularExpression(name, "i"))
        );
        return await Collection.Find(filter).ToListAsync();
    }
}
