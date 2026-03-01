namespace HospitalCare.Domain.Entities;

public class Doctor : Entity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Specialization { get; private set; }
    public string Email { get; private set; }
    public string Phone { get; private set; }
    public string LicenseNumber { get; private set; }

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Doctor() { }

    public Doctor(string firstName, string lastName, string specialization, string email, string phone, string licenseNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Specialization = specialization;
        Email = email;
        Phone = phone;
        LicenseNumber = licenseNumber;
    }
}
