using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalCare.Application.Validators;

public interface IValidationService
{
    Task<ValidationResult> ValidateAsync<T>(T instance);
}

public class ValidationService : IValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ValidationResult> ValidateAsync<T>(T instance)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator == null)
        {
            return new ValidationResult();
        }
        
        return await validator.ValidateAsync(instance);
    }
}
