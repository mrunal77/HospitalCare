using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdateBioValidator : AbstractValidator<UpdateBioDto>
{
    private const int MaxBioLength = 160;

    public UpdateBioValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(MaxBioLength)
            .WithMessage($"Bio cannot exceed {MaxBioLength} characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));
    }
}
