using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoRepository<T> : IRepository<T> where T : class
{
    protected readonly IMongoCollection<T> Collection;

    public MongoRepository(MongoDbContext context)
    {
        Collection = context.GetCollection<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("_id", new BsonBinaryData(id, GuidRepresentation.Standard));
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Collection.Find(_ => true).ToListAsync();
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null)
        {
            var id = (Guid)idProperty.GetValue(entity)!;
            var filter = Builders<T>.Filter.Eq("_id", new BsonBinaryData(id, GuidRepresentation.Standard));
            await Collection.ReplaceOneAsync(filter, entity);
        }
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq("_id", new BsonBinaryData(id, GuidRepresentation.Standard));
        await Collection.DeleteOneAsync(filter);
    }
}