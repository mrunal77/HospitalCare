using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class UpdatePreferencesValidator : AbstractValidator<UpdatePreferencesDto>
{
    private static readonly string[] SupportedLanguages = 
    { 
        "en-US", "es-ES", "fr-FR", "de-DE", "ja-JP", "zh-CN", "pt-BR", "it-IT", "ko-KR", "ar-SA" 
    };

    public UpdatePreferencesValidator()
    {
        RuleFor(x => x.Timezone)
            .NotEmpty()
            .WithMessage("Timezone is required")
            .Must(BeValidTimezone)
            .WithMessage("Invalid timezone. Please select a valid IANA timezone identifier (e.g., America/New_York, Europe/London, Asia/Tokyo)");

        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage("Language is required")
            .Must(x => SupportedLanguages.Contains(x))
            .WithMessage($"Invalid language. Supported languages: {string.Join(", ", SupportedLanguages)}");
    }

    private static bool BeValidTimezone(string timezone)
    {
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
