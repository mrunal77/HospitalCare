using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class RescheduleAppointmentValidator : AbstractValidator<RescheduleAppointmentDto>
{
    public RescheduleAppointmentValidator()
    {
        RuleFor(x => x.NewDate)
            .NotEmpty()
            .WithMessage("New date is required")
            .Must(BeInTheFuture)
            .WithMessage("New appointment date must be in the future");

        RuleFor(x => x.NewDurationMinutes)
            .NotEmpty()
            .WithMessage("Duration is required")
            .InclusiveBetween(15, 180)
            .WithMessage("Duration must be between 15 and 180 minutes");
    }

    private static bool BeInTheFuture(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
