using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data;
using HospitalCare.Infrastructure.Data.MongoDB;
using HospitalCare.Infrastructure.Repositories.MongoDB;
using HospitalCare.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using StackExchange.Redis;

namespace HospitalCare.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetConnectionString("MongoDB") 
            ?? configuration["MongoDB:ConnectionString"] 
            ?? "mongodb://localhost:27017";
        var mongoDatabaseName = configuration["MongoDB:DatabaseName"] ?? "HospitalCareDb";

        var redisConnectionString = configuration.GetConnectionString("Redis")
            ?? configuration["Redis:ConnectionString"]
            ?? "localhost:6379";

        var retryPolicy = GetRetryPolicy();

        MongoDbMappings.RegisterMappings();

        services.AddSingleton<MongoDbContext>(sp => 
        {
            var policy = retryPolicy;
            return policy.Execute(() => new MongoDbContext(mongoConnectionString, mongoDatabaseName));
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var policy = retryPolicy;
            return policy.Execute(() => ConnectionMultiplexer.Connect(redisConnectionString));
        });

        services.AddScoped<IPatientRepository, MongoPatientRepository>();
        services.AddScoped<IDoctorRepository, MongoDoctorRepository>();
        services.AddScoped<IAppointmentRepository, MongoAppointmentRepository>();
        services.AddScoped<IPrescriptionRepository, MongoPrescriptionRepository>();
        services.AddScoped<IMedicineRepository, MongoMedicineRepository>();
        services.AddScoped<IUserRepository, MongoUserRepository>();
        services.AddScoped<IRoleRepository, MongoRoleRepository>();
        services.AddScoped<IClaimRepository, MongoClaimRepository>();
        services.AddScoped<IUserClaimRepository, MongoUserClaimRepository>();
        services.AddScoped<IRoleClaimRepository, MongoRoleClaimRepository>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }

    private static ResiliencePipeline GetRetryPolicy()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder().Handle<MongoConnectionException>()
                    .Handle<MongoCommandException>()
                    .Handle<RedisConnectionException>()
                    .Handle<RedisTimeoutException>(),
                OnRetry = args =>
                {
                    Console.WriteLine($"Retry attempt {args.AttemptNumber} due to: {args.Outcome.Exception?.Message}");
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await serviceProvider.CreateMongoIndexesAsync();
        var context = serviceProvider.GetRequiredService<MongoDbContext>();
        await DatabaseSeeder.SeedAsync(context);
    }

    private static async Task CreateMongoIndexesAsync(this IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<MongoDbContext>();

        try
        {
            await context.Patients.Indexes.DropAllAsync();
            await context.Patients.Indexes.CreateOneAsync(
                new CreateIndexModel<Domain.Entities.Patient>(
                    Builders<Domain.Entities.Patient>.IndexKeys.Ascending(p => p.Email),
                    new CreateIndexOptions { Unique = true, Sparse = true, Name = "idx_patient_email" }
                )
            );
        }
        catch (MongoCommandException) { }

        try
        {
            await context.Doctors.Indexes.DropAllAsync();
            await context.Doctors.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Domain.Entities.Doctor>(
                    Builders<Domain.Entities.Doctor>.IndexKeys.Ascending(d => d.LicenseNumber),
                    new CreateIndexOptions { Unique = true, Sparse = true, Name = "idx_doctor_license" }
                ),
                new CreateIndexModel<Domain.Entities.Doctor>(
                    Builders<Domain.Entities.Doctor>.IndexKeys.Ascending(d => d.Specialization),
                    new CreateIndexOptions { Name = "idx_doctor_specialization" }
                )
            });
        }
        catch (MongoCommandException) { }

        try
        {
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
        }
        catch (MongoCommandException) { }

        try
        {
            await context.Prescriptions.Indexes.DropAllAsync();
            await context.Prescriptions.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Domain.Entities.Prescription>(
                    Builders<Domain.Entities.Prescription>.IndexKeys.Ascending(p => p.PatientId),
                    new CreateIndexOptions { Name = "idx_prescription_patient" }
                ),
                new CreateIndexModel<Domain.Entities.Prescription>(
                    Builders<Domain.Entities.Prescription>.IndexKeys.Ascending(p => p.DoctorId),
                    new CreateIndexOptions { Name = "idx_prescription_doctor" }
                ),
                new CreateIndexModel<Domain.Entities.Prescription>(
                    Builders<Domain.Entities.Prescription>.IndexKeys.Ascending(p => p.AppointmentId),
                    new CreateIndexOptions { Name = "idx_prescription_appointment" }
                )
            });
        }
        catch (MongoCommandException) { }

        try
        {
            await context.Medicines.Indexes.DropAllAsync();
            await context.Medicines.Indexes.CreateManyAsync(new[]
            {
                new CreateIndexModel<Domain.Entities.Medicine>(
                    Builders<Domain.Entities.Medicine>.IndexKeys.Ascending(m => m.ProductName),
                    new CreateIndexOptions { Name = "idx_medicine_name" }
                ),
                new CreateIndexModel<Domain.Entities.Medicine>(
                    Builders<Domain.Entities.Medicine>.IndexKeys.Ascending(m => m.SubCategory),
                    new CreateIndexOptions { Name = "idx_medicine_category" }
                ),
                new CreateIndexModel<Domain.Entities.Medicine>(
                    Builders<Domain.Entities.Medicine>.IndexKeys.Ascending(m => m.SaltComposition),
                    new CreateIndexOptions { Name = "idx_medicine_salt" }
                ),
                new CreateIndexModel<Domain.Entities.Medicine>(
                    Builders<Domain.Entities.Medicine>.IndexKeys.Ascending(m => m.Manufacturer),
                    new CreateIndexOptions { Name = "idx_medicine_manufacturer" }
                )
            });
        }
        catch (MongoCommandException) { }

        try
        {
            await context.Users.Indexes.DropAllAsync();
            await context.Users.Indexes.CreateOneAsync(
                new CreateIndexModel<Domain.Entities.User>(
                    Builders<Domain.Entities.User>.IndexKeys.Ascending(u => u.Email),
                    new CreateIndexOptions { Unique = true, Sparse = true, Name = "idx_user_email" }
                )
            );
            await context.Users.Indexes.CreateOneAsync(
                new CreateIndexModel<Domain.Entities.User>(
                    Builders<Domain.Entities.User>.IndexKeys.Ascending(u => u.RoleId),
                    new CreateIndexOptions { Name = "idx_user_role" }
                )
            );
        }
        catch (MongoCommandException) { }

        try
        {
            await context.Roles.Indexes.DropAllAsync();
            await context.Roles.Indexes.CreateOneAsync(
                new CreateIndexModel<Domain.Entities.Role>(
                    Builders<Domain.Entities.Role>.IndexKeys.Ascending(r => r.Name),
                    new CreateIndexOptions { Unique = true, Sparse = true, Name = "idx_role_name" }
                )
            );
        }
        catch (MongoCommandException) { }
    }
}
