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
    public IMongoCollection<Domain.Entities.Prescription> Prescriptions => GetCollection<Domain.Entities.Prescription>("prescriptions");
    public IMongoCollection<Domain.Entities.Medicine> Medicines => GetCollection<Domain.Entities.Medicine>("medicines");
    public IMongoCollection<Domain.Entities.User> Users => GetCollection<Domain.Entities.User>("users");
    public IMongoCollection<Domain.Entities.Role> Roles => GetCollection<Domain.Entities.Role>("roles");
    public IMongoCollection<Domain.Entities.Claim> Claims => GetCollection<Domain.Entities.Claim>("claims");
    public IMongoCollection<Domain.Entities.UserClaim> UserClaims => GetCollection<Domain.Entities.UserClaim>("userclaims");
    public IMongoCollection<Domain.Entities.RoleClaim> RoleClaims => GetCollection<Domain.Entities.RoleClaim>("roleclaims");
    public IMongoCollection<Specialization> Specializations => GetCollection<Specialization>("specializations");
}

public class Specialization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
