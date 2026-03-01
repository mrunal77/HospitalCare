using FluentValidation;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Application.Services;
using HospitalCare.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalCare.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddValidatorsFromAssemblyContaining<UpdateProfileValidator>();
        services.AddScoped<IValidationService, ValidationService>();

        return services;
    }
}
