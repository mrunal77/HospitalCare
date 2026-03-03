namespace HospitalCare.Domain.Entities;

public class Medicine : Entity
{
    public string SubCategory { get; private set; }
    public string ProductName { get; private set; }
    public string SaltComposition { get; private set; }
    public decimal? Price { get; private set; }
    public string Manufacturer { get; private set; }
    public string Description { get; private set; }
    public string? SideEffects { get; private set; }
    public string? DrugInteractions { get; private set; }
    public bool IsActive { get; private set; }

    private Medicine() { }

    public Medicine(
        string subCategory,
        string productName,
        string saltComposition,
        decimal? price,
        string manufacturer,
        string description,
        string? sideEffects = null,
        string? drugInteractions = null)
    {
        SubCategory = subCategory;
        ProductName = productName;
        SaltComposition = saltComposition;
        Price = price;
        Manufacturer = manufacturer;
        Description = description;
        SideEffects = sideEffects;
        DrugInteractions = drugInteractions;
        IsActive = true;
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

    public void UpdatePrice(decimal price)
    {
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }
}
