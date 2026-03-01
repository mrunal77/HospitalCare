using HospitalCare.Domain.Entities;

namespace HospitalCare.Domain.Events;

public record AppointmentCreatedEvent(Appointment Appointment) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
