namespace HospitalCare.Domain.Entities;

public class Appointment : Entity
{
    public Guid PatientId { get; private set; }
    public Guid DoctorId { get; private set; }
    public DateTime AppointmentDate { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string Reason { get; private set; }
    public string? Notes { get; private set; }
    public AppointmentStatus Status { get; private set; }

    public Patient Patient { get; private set; } = null!;
    public Doctor Doctor { get; private set; } = null!;

    private Appointment() { }

    public Appointment(Guid patientId, Guid doctorId, DateTime appointmentDate, TimeSpan duration, string reason, string? notes = null)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentDate = appointmentDate;
        Duration = duration;
        Reason = reason;
        Notes = notes;
        Status = AppointmentStatus.Scheduled;
    }

    public void Cancel(string? reason = null)
    {
        Status = AppointmentStatus.Cancelled;
        Notes = string.IsNullOrEmpty(reason) ? Notes : $"{Notes}\nCancelled: {reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string? notes = null)
    {
        Status = AppointmentStatus.Completed;
        Notes = string.IsNullOrEmpty(notes) ? Notes : $"{Notes}\n{notes}";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reschedule(DateTime newDate, TimeSpan newDuration)
    {
        AppointmentDate = newDate;
        Duration = newDuration;
        Status = AppointmentStatus.Rescheduled;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum AppointmentStatus
{
    Scheduled,
    Completed,
    Cancelled,
    Rescheduled
}
