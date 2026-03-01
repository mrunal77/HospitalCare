using FluentValidation;
using HospitalCare.Application.DTOs;

namespace HospitalCare.Application.Validators;

public class CreateAppointmentValidator : AbstractValidator<CreateAppointmentDto>
{
    public CreateAppointmentValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty()
            .WithMessage("Patient ID is required");

        RuleFor(x => x.DoctorId)
            .NotEmpty()
            .WithMessage("Doctor ID is required")
            .NotEqual(x => x.PatientId)
            .WithMessage("Doctor and Patient cannot be the same");

        RuleFor(x => x.AppointmentDate)
            .NotEmpty()
            .WithMessage("Appointment date is required")
            .Must(BeInTheFuture)
            .WithMessage("Appointment date must be in the future");

        RuleFor(x => x.DurationMinutes)
            .NotEmpty()
            .WithMessage("Duration is required")
            .InclusiveBetween(15, 180)
            .WithMessage("Duration must be between 15 and 180 minutes");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }

    private static bool BeInTheFuture(DateTime date)
    {
        return date > DateTime.UtcNow;
    }
}
