using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdatePatientValidator : AbstractValidator<UpdatePatientDto>
{
    public UpdatePatientValidator()
    {
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
}
