using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdateUsernameValidator : AbstractValidator<UpdateUsernameDto>
{
    public UpdateUsernameValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .MinimumLength(3)
            .WithMessage("Username must be at least 3 characters")
            .MaximumLength(20)
            .WithMessage("Username must be at most 20 characters")
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.DisplayName)
            .MaximumLength(50)
            .WithMessage("Display name cannot exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.DisplayName));
    }
}
