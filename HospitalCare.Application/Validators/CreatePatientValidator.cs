using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class CreatePatientValidator : AbstractValidator<CreatePatientDto>
{
    public CreatePatientValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .WithMessage("Date of birth is required")
            .Must(BeAValidDate)
            .WithMessage("Date of birth must be a valid date")
            .Must(BeInThePast)
            .WithMessage("Date of birth must be in the past")
            .Must(BeAtLeast18YearsOld)
            .WithMessage("Patient must be at least 18 years old");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Please enter a valid email address");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\+?[\d\s\-()]+$")
            .WithMessage("Please enter a valid phone number")
            .MinimumLength(10)
            .WithMessage("Phone number must be at least 10 digits");

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Address));
    }

    private static bool BeAValidDate(DateTime date)
    {
        return date != default;
    }

    private static bool BeInThePast(DateTime date)
    {
        return date < DateTime.UtcNow;
    }

    private static bool BeAtLeast18YearsOld(DateTime dateOfBirth)
    {
        var age = DateTime.UtcNow.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.UtcNow.AddYears(-age)) age--;
        return age >= 18;
    }
}
