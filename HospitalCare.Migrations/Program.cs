using HospitalCare.Infrastructure.Migrations;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connectionString = configuration.GetConnectionString("MongoDB") 
    ?? configuration["MongoDB:ConnectionString"] 
    ?? "mongodb://localhost:27017";
var databaseName = configuration["MongoDB:DatabaseName"] ?? "HospitalCareDb";

Console.WriteLine($"Connecting to MongoDB: {connectionString}");
Console.WriteLine($"Database: {databaseName}");
Console.WriteLine();

await MigrationRunner.RunMigrationsAsync(connectionString, databaseName);
