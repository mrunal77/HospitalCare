using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class CreateDoctorValidator : AbstractValidator<CreateDoctorDto>
{
    public CreateDoctorValidator()
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

        RuleFor(x => x.Specialization)
            .NotEmpty()
            .WithMessage("Specialization is required")
            .MaximumLength(100)
            .WithMessage("Specialization cannot exceed 100 characters");

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

        RuleFor(x => x.LicenseNumber)
            .NotEmpty()
            .WithMessage("License number is required")
            .MaximumLength(50)
            .WithMessage("License number cannot exceed 50 characters");
    }
}
