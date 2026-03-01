using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoRoleRepository : MongoRepository<Role>, IRoleRepository
{
    public MongoRoleRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(r => r.Name, name);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        var filter = Builders<Role>.Filter.Eq(r => r.Name, name);
        return await Collection.Find(filter).AnyAsync();
    }
}
