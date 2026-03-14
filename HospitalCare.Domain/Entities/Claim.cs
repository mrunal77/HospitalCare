namespace HospitalCare.Domain.Entities;

public class Claim : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Category { get; private set; }
    public bool IsActive { get; private set; }

    private Claim() { }

    public Claim(string name, string description, string category)
    {
        Name = name;
        Description = description;
        Category = category;
        IsActive = true;
    }

    public void Update(string name, string description, string category)
    {
        Name = name;
        Description = description;
        Category = category;
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
