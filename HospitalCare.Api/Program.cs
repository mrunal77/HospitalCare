using System.Text;
using HospitalCare.Api.Middleware;
using HospitalCare.Application;
using HospitalCare.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") 
    ?? "mongodb://localhost:27017";
var mongoDatabaseName = "HospitalCareDb";

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilogLogging(mongoConnectionString, mongoDatabaseName);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:3000",
                    "http://localhost:4200",
                    "http://127.0.0.1:3000",
                    "http://192.168.31.248:3000"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "HospitalCare_SuperSecretKey_2024_VeryLongKey!";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "HospitalCare";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "HospitalCareApi";

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
        };
    });

    builder.Services.AddAuthorization();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    using (var scope = app.Services.CreateScope())
    {
        await scope.ServiceProvider.InitializeDatabaseAsync();
    }

    app.MapOpenApi();
    app.MapScalarApiReference();

    app.UseCors("AllowReactApp");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Serilog.Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    SerilogMiddleware.CloseAndFlushSerilog();
}
