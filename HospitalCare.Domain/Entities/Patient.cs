namespace HospitalCare.Domain.Entities;

public class Patient : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string? Address { get; private set; }

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Patient() { }

    public Patient(string firstName, string lastName, DateTime dateOfBirth, string email, string phone, string? address = null)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        Phone = phone;
        Address = address;
    }

    public void UpdateContactInfo(string email, string phone, string? address = null)
    {
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
}
