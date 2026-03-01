using HospitalCare.Domain.Entities;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace HospitalCare.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        await SeedRolesAsync(context);
        await MigrateUsersAsync(context);
        await SeedSampleDataAsync(context);
    }

    private static async Task SeedRolesAsync(MongoDbContext context)
    {
        var roleCount = await context.Roles.CountDocumentsAsync(_ => true);
        if (roleCount == 0)
        {
            var roles = new List<Role>
            {
                new("Admin", "System administrator with full access", "FullAccess"),
                new("Doctor", "Medical doctor with patient access", "DoctorAccess"),
                new("HospitalEmployee", "Hospital staff with limited access", "EmployeeAccess"),
                new("Nurse", "Nursing staff with patient care access", "NurseAccess"),
                new("Receptionist", "Front desk staff with appointment access", "ReceptionAccess")
            };

            try
            {
                await context.Roles.InsertManyAsync(roles);
                Console.WriteLine($"Seeded {roles.Count} default roles");
                foreach (var role in roles)
                {
                    Console.WriteLine($"  - {role.Name}: {role.Id}");
                }
            }
            catch (MongoBulkWriteException) { }
        }
    }

    private static async Task MigrateUsersAsync(MongoDbContext context)
    {
        var adminRole = await context.Roles.Find(r => r.Name == "Admin").FirstOrDefaultAsync();
        var employeeRole = await context.Roles.Find(r => r.Name == "HospitalEmployee").FirstOrDefaultAsync();

        if (adminRole is null || employeeRole is null) return;

        var usersCollection = context.GetCollection<BsonDocument>("users");
        
        var existingAdmin = await usersCollection.Find(new BsonDocument("email", "admin@hospitalcare.com")).FirstOrDefaultAsync();
        
        if (existingAdmin is null)
        {
            var adminDoc = new BsonDocument
            {
                { "_id", new BsonBinaryData(Guid.NewGuid(), GuidRepresentation.Standard) },
                { "email", "admin@hospitalcare.com" },
                { "passwordHash", HashPassword("Admin@123") },
                { "firstName", "System" },
                { "lastName", "Administrator" },
                { "roleId", new BsonBinaryData(adminRole.Id, GuidRepresentation.Standard) },
                { "isActive", true },
                { "createdAt", DateTime.UtcNow },
                { "updatedAt", DateTime.UtcNow }
            };
            await usersCollection.InsertOneAsync(adminDoc);
            Console.WriteLine("Admin user created: admin@hospitalcare.com / Admin@123");
        }
        else
        {
            var update = new BsonDocument
            {
                { "$set", new BsonDocument
                    {
                        { "passwordHash", HashPassword("Admin@123") },
                        { "firstName", "System" },
                        { "lastName", "Administrator" },
                        { "roleId", new BsonBinaryData(adminRole.Id, GuidRepresentation.Standard) },
                        { "isActive", true },
                        { "updatedAt", DateTime.UtcNow }
                    }
                }
            };
            await usersCollection.UpdateOneAsync(new BsonDocument("email", "admin@hospitalcare.com"), update);
            Console.WriteLine("Admin user reset: admin@hospitalcare.com / Admin@123");
        }

        var existingEmployee = await usersCollection.Find(new BsonDocument("email", "reception@hospitalcare.com")).FirstOrDefaultAsync();
        
        if (existingEmployee is null && employeeRole is not null)
        {
            var employeeDoc = new BsonDocument
            {
                { "_id", new BsonBinaryData(Guid.NewGuid(), GuidRepresentation.Standard) },
                { "email", "reception@hospitalcare.com" },
                { "passwordHash", HashPassword("Employee@123") },
                { "firstName", "Jane" },
                { "lastName", "Reception" },
                { "roleId", new BsonBinaryData(employeeRole.Id, GuidRepresentation.Standard) },
                { "isActive", true },
                { "createdAt", DateTime.UtcNow },
                { "updatedAt", DateTime.UtcNow }
            };
            await usersCollection.InsertOneAsync(employeeDoc);
            Console.WriteLine("Employee user created: reception@hospitalcare.com / Employee@123");
        }
        else if (existingEmployee is not null && employeeRole is not null)
        {
            var update = new BsonDocument
            {
                { "$set", new BsonDocument
                    {
                        { "roleId", new BsonBinaryData(employeeRole.Id, GuidRepresentation.Standard) },
                        { "updatedAt", DateTime.UtcNow }
                    }
                }
            };
            await usersCollection.UpdateOneAsync(new BsonDocument("email", "reception@hospitalcare.com"), update);
        }
    }

    private static async Task SeedSampleDataAsync(MongoDbContext context)
    {
        var doctorCount = await context.Doctors.CountDocumentsAsync(_ => true);
        if (doctorCount == 0)
        {
            var doctors = new List<Doctor>
            {
                new("John", "Smith", "Cardiology", "john.smith@hospitalcare.com", "+1-555-0101", "DOC-001"),
                new("Sarah", "Johnson", "Neurology", "sarah.johnson@hospitalcare.com", "+1-555-0102", "DOC-002"),
                new("Michael", "Williams", "Orthopedics", "michael.williams@hospitalcare.com", "+1-555-0103", "DOC-003"),
                new("Emily", "Brown", "Pediatrics", "emily.brown@hospitalcare.com", "+1-555-0104", "DOC-004"),
                new("David", "Davis", "Oncology", "david.davis@hospitalcare.com", "+1-555-0105", "DOC-005")
            };

            try
            {
                await context.Doctors.InsertManyAsync(doctors);
                Console.WriteLine($"Seeded {doctors.Count} sample doctors");
            }
            catch (MongoBulkWriteException) { }
        }

        var patientCount = await context.Patients.CountDocumentsAsync(_ => true);
        if (patientCount == 0)
        {
            var patients = new List<Patient>
            {
                new("Alice", "Thompson", new DateTime(1985, 3, 15), "alice.t@email.com", "+1-555-1001", "123 Main St, City"),
                new("Robert", "Anderson", new DateTime(1972, 7, 22), "robert.a@email.com", "+1-555-1002", "456 Oak Ave, Town"),
                new("Lisa", "Martinez", new DateTime(1990, 11, 8), "lisa.m@email.com", "+1-555-1003", "789 Pine Rd, Village"),
                new("James", "Wilson", new DateTime(1968, 1, 30), "james.w@email.com", "+1-555-1004", "321 Elm St, Metro"),
                new("Maria", "Garcia", new DateTime(1995, 5, 12), "maria.g@email.com", "+1-555-1005", "654 Cedar Ln, County")
            };

            try
            {
                await context.Patients.InsertManyAsync(patients);
                Console.WriteLine($"Seeded {patients.Count} sample patients");
            }
            catch (MongoBulkWriteException) { }
        }
    }

    private static string HashPassword(string password)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000, HashAlgorithmName.SHA256, 32);
        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }
}
