namespace HospitalCare.Domain.Entities;

public class User : Entity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Guid RoleId { get; private set; }
    public bool IsActive { get; private set; }

    internal User() { }

    public User(string email, string passwordHash, string firstName, string lastName, Guid roleId)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        RoleId = roleId;
        IsActive = true;
    }

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}
