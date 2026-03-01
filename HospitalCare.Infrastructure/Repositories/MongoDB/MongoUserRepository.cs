using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoUserRepository : MongoRepository<User>, IUserRepository
{
    public MongoUserRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Role, role);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await Collection.Find(filter).AnyAsync();
    }
}
