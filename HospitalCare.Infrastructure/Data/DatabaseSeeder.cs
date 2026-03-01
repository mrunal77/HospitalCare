using HospitalCare.Domain.Entities;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace HospitalCare.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MongoDbContext context)
    {
        await SeedAdminUserAsync(context);
        await SeedSampleDataAsync(context);
    }

    private static async Task SeedAdminUserAsync(MongoDbContext context)
    {
        var existingAdmin = await context.Users
            .Find(u => u.Email == "admin@hospitalcare.com")
            .FirstOrDefaultAsync();

        if (existingAdmin is null)
        {
            var adminUser = new User(
                "admin@hospitalcare.com",
                HashPassword("Admin@123"),
                "System",
                "Administrator",
                UserRole.Admin
            );
            await context.Users.InsertOneAsync(adminUser);
            Console.WriteLine("Admin user created: admin@hospitalcare.com / Admin@123");
        }
        else
        {
            var passwordHash = HashPassword("Admin@123");
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, passwordHash)
                .Set(u => u.FirstName, "System")
                .Set(u => u.LastName, "Administrator")
                .Set(u => u.Role, UserRole.Admin)
                .Set(u => u.IsActive, true);

            await context.Users.UpdateOneAsync(u => u.Email == "admin@hospitalcare.com", update);
            Console.WriteLine("Admin user reset: admin@hospitalcare.com / Admin@123");
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

        var employeeCount = await context.Users.CountDocumentsAsync(u => u.Role == UserRole.HospitalEmployee);
        if (employeeCount == 0)
        {
            var employees = new List<User>
            {
                new("reception@hospitalcare.com", HashPassword("Employee@123"), "Jane", "Reception", UserRole.HospitalEmployee),
                new("nurse@hospitalcare.com", HashPassword("Employee@123"), "Mark", "Nurse", UserRole.HospitalEmployee)
            };

            try
            {
                await context.Users.InsertManyAsync(employees);
                Console.WriteLine($"Seeded {employees.Count} sample employees");
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
