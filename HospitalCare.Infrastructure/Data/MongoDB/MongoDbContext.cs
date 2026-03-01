using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Data.MongoDB;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string? collectionName = null)
    {
        return _database.GetCollection<T>(collectionName ?? typeof(T).Name.ToLower() + "s");
    }

    public IMongoCollection<Domain.Entities.Patient> Patients => GetCollection<Domain.Entities.Patient>("patients");
    public IMongoCollection<Domain.Entities.Doctor> Doctors => GetCollection<Domain.Entities.Doctor>("doctors");
    public IMongoCollection<Domain.Entities.Appointment> Appointments => GetCollection<Domain.Entities.Appointment>("appointments");
    public IMongoCollection<Domain.Entities.User> Users => GetCollection<Domain.Entities.User>("users");
    public IMongoCollection<Domain.Entities.Role> Roles => GetCollection<Domain.Entities.Role>("roles");
}
