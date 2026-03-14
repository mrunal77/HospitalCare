using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoRoleClaimRepository : MongoRepository<RoleClaim>, IRoleClaimRepository
{
    private readonly IMongoCollection<Claim> _claimsCollection;

    public MongoRoleClaimRepository(MongoDbContext context) : base(context)
    {
        _claimsCollection = context.GetCollection<Claim>();
    }

    public async Task<IEnumerable<RoleClaim>> GetByRoleIdAsync(Guid roleId)
    {
        var filter = Builders<RoleClaim>.Filter.Eq(rc => rc.RoleId, roleId);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Claim>> GetClaimsByRoleIdAsync(Guid roleId)
    {
        var roleClaimFilter = Builders<RoleClaim>.Filter.Eq(rc => rc.RoleId, roleId);
        var roleClaims = await Collection.Find(roleClaimFilter).ToListAsync();
        var claimIds = roleClaims.Select(rc => rc.ClaimId).ToList();

        if (!claimIds.Any())
            return Enumerable.Empty<Claim>();

        var claimFilter = Builders<Claim>.Filter.In(c => c.Id, claimIds);
        return await _claimsCollection.Find(claimFilter).ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid roleId, Guid claimId)
    {
        var filter = Builders<RoleClaim>.Filter.And(
            Builders<RoleClaim>.Filter.Eq(rc => rc.RoleId, roleId),
            Builders<RoleClaim>.Filter.Eq(rc => rc.ClaimId, claimId)
        );
        return await Collection.Find(filter).AnyAsync();
    }

    public async Task DeleteByRoleIdAsync(Guid roleId)
    {
        var filter = Builders<RoleClaim>.Filter.Eq(rc => rc.RoleId, roleId);
        await Collection.DeleteManyAsync(filter);
    }

    public async Task DeleteAsync(Guid roleId, Guid claimId)
    {
        var filter = Builders<RoleClaim>.Filter.And(
            Builders<RoleClaim>.Filter.Eq(rc => rc.RoleId, roleId),
            Builders<RoleClaim>.Filter.Eq(rc => rc.ClaimId, claimId)
        );
        await Collection.DeleteOneAsync(filter);
    }
}
