using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoUserClaimRepository : MongoRepository<UserClaim>, IUserClaimRepository
{
    private readonly IMongoCollection<Claim> _claimsCollection;

    public MongoUserClaimRepository(MongoDbContext context) : base(context)
    {
        _claimsCollection = context.GetCollection<Claim>();
    }

    public async Task<IEnumerable<UserClaim>> GetByUserIdAsync(Guid userId)
    {
        var filter = Builders<UserClaim>.Filter.Eq(uc => uc.UserId, userId);
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Claim>> GetClaimsByUserIdAsync(Guid userId)
    {
        var userClaimFilter = Builders<UserClaim>.Filter.Eq(uc => uc.UserId, userId);
        var userClaims = await Collection.Find(userClaimFilter).ToListAsync();
        var claimIds = userClaims.Select(uc => uc.ClaimId).ToList();

        if (!claimIds.Any())
            return Enumerable.Empty<Claim>();

        var claimFilter = Builders<Claim>.Filter.In(c => c.Id, claimIds);
        return await _claimsCollection.Find(claimFilter).ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid claimId)
    {
        var filter = Builders<UserClaim>.Filter.And(
            Builders<UserClaim>.Filter.Eq(uc => uc.UserId, userId),
            Builders<UserClaim>.Filter.Eq(uc => uc.ClaimId, claimId)
        );
        return await Collection.Find(filter).AnyAsync();
    }

    public async Task DeleteByUserIdAsync(Guid userId)
    {
        var filter = Builders<UserClaim>.Filter.Eq(uc => uc.UserId, userId);
        await Collection.DeleteManyAsync(filter);
    }

    public async Task DeleteAsync(Guid userId, Guid claimId)
    {
        var filter = Builders<UserClaim>.Filter.And(
            Builders<UserClaim>.Filter.Eq(uc => uc.UserId, userId),
            Builders<UserClaim>.Filter.Eq(uc => uc.ClaimId, claimId)
        );
        await Collection.DeleteOneAsync(filter);
    }
}
