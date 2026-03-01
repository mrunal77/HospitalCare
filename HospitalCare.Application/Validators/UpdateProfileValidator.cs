using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileDto>
{
    private const int MaxBioLength = 160;
    private const int MaxNameLength = 100;

    private static readonly string[] SupportedLanguages = 
    { 
        "en-US", "es-ES", "fr-FR", "de-DE", "ja-JP", "zh-CN", "pt-BR", "it-IT", "ko-KR", "ar-SA" 
    };

    public UpdateProfileValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(MaxNameLength)
            .WithMessage($"First name cannot exceed {MaxNameLength} characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(MaxNameLength)
            .WithMessage($"Last name cannot exceed {MaxNameLength} characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.Bio)
            .MaximumLength(MaxBioLength)
            .WithMessage($"Bio cannot exceed {MaxBioLength} characters")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.Timezone)
            .Must(BeValidTimezone)
            .WithMessage("Invalid timezone. Please select a valid IANA timezone identifier")
            .When(x => !string.IsNullOrEmpty(x.Timezone));

        RuleFor(x => x.Language)
            .Must(x => SupportedLanguages.Contains(x))
            .WithMessage($"Invalid language. Supported: {string.Join(", ", SupportedLanguages)}")
            .When(x => !string.IsNullOrEmpty(x.Language));
    }

    private static bool BeValidTimezone(string? timezone)
    {
        if (string.IsNullOrEmpty(timezone))
            return true;
            
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timezone);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
