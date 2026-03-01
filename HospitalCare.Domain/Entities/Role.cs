namespace HospitalCare.Domain.Entities;

public class Role : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Permission { get; private set; }
    public bool IsActive { get; private set; }

    private Role() { }

    public Role(string name, string description, string permission)
    {
        Name = name;
        Description = description;
        Permission = permission;
        IsActive = true;
    }

    public void Update(string name, string description, string permission)
    {
        Name = name;
        Description = description;
        Permission = permission;
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
}
