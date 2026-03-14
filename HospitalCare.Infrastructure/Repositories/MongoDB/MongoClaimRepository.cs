using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoClaimRepository : MongoRepository<Claim>, IClaimRepository
{
    public MongoClaimRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<Claim?> GetByNameAsync(string name)
    {
        var filter = Builders<Claim>.Filter.Eq(c => c.Name, name);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Claim>> GetActiveAsync()
    {
        var filter = Builders<Claim>.Filter.Eq(c => c.IsActive, true);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Claim>> GetByCategoryAsync(string category)
    {
        var filter = Builders<Claim>.Filter.Eq(c => c.Category, category);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        var filter = Builders<Claim>.Filter.Eq(c => c.Name, name);
        return await Collection.Find(filter).AnyAsync();
    }
}
