using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;
using HospitalCare.Infrastructure.Data;

namespace HospitalCare.Infrastructure.Migrations;

public static class MigrationRunner
{
    public static async Task RunMigrationsAsync(string connectionString, string databaseName)
    {
        Console.WriteLine("Starting MongoDB migrations...");
        
        var context = new MongoDbContext(connectionString, databaseName);
        
        Console.WriteLine("Creating indexes...");
        await CreateIndexesAsync(context);
        
        Console.WriteLine("Seeding initial data...");
        await DatabaseSeeder.SeedAsync(context);
        
        Console.WriteLine("Migrations completed successfully!");
    }

    private static async Task CreateIndexesAsync(MongoDbContext context)
    {
        Console.WriteLine("  Creating Patient indexes...");
        await context.Patients.Indexes.DropAllAsync();
        await context.Patients.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Domain.Entities.Patient>(
                Builders<Domain.Entities.Patient>.IndexKeys.Ascending(p => p.Email),
                new CreateIndexOptions { Unique = true, Name = "idx_patient_email" }
            ),
            new CreateIndexModel<Domain.Entities.Patient>(
                Builders<Domain.Entities.Patient>.IndexKeys.Text(p => p.FirstName).Text(p => p.LastName),
                new CreateIndexOptions { Name = "idx_patient_name_text" }
            )
        });

        Console.WriteLine("  Creating Doctor indexes...");
        await context.Doctors.Indexes.DropAllAsync();
        await context.Doctors.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Domain.Entities.Doctor>(
                Builders<Domain.Entities.Doctor>.IndexKeys.Ascending(d => d.LicenseNumber),
                new CreateIndexOptions { Unique = true, Name = "idx_doctor_license" }
            ),
            new CreateIndexModel<Domain.Entities.Doctor>(
                Builders<Domain.Entities.Doctor>.IndexKeys.Ascending(d => d.Specialization),
                new CreateIndexOptions { Name = "idx_doctor_specialization" }
            )
        });

        Console.WriteLine("  Creating Appointment indexes...");
        await context.Appointments.Indexes.DropAllAsync();
        await context.Appointments.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Domain.Entities.Appointment>(
                Builders<Domain.Entities.Appointment>.IndexKeys.Ascending(a => a.PatientId),
                new CreateIndexOptions { Name = "idx_appointment_patient" }
            ),
            new CreateIndexModel<Domain.Entities.Appointment>(
                Builders<Domain.Entities.Appointment>.IndexKeys.Ascending(a => a.DoctorId),
                new CreateIndexOptions { Name = "idx_appointment_doctor" }
            ),
            new CreateIndexModel<Domain.Entities.Appointment>(
                Builders<Domain.Entities.Appointment>.IndexKeys.Ascending(a => a.AppointmentDate),
                new CreateIndexOptions { Name = "idx_appointment_date" }
            )
        });

        Console.WriteLine("  Creating User indexes...");
        await context.Users.Indexes.DropAllAsync();
        await context.Users.Indexes.CreateManyAsync(new[]
        {
            new CreateIndexModel<Domain.Entities.User>(
                Builders<Domain.Entities.User>.IndexKeys.Ascending(u => u.Email),
                new CreateIndexOptions { Unique = true, Name = "idx_user_email" }
            )
        });
    }
}
