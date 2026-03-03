using HospitalCare.Domain.Entities;
using HospitalCare.Domain.Interfaces.Repositories;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Repositories.MongoDB;

public class MongoMedicineRepository : MongoRepository<Medicine>, IMedicineRepository
{
    public MongoMedicineRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Medicine>> GetBySubCategoryAsync(string subCategory)
    {
        var filter = Builders<Medicine>.Filter.Eq(m => m.SubCategory, subCategory);
        var sort = Builders<Medicine>.Sort.Ascending(m => m.ProductName);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<Medicine>> SearchByNameAsync(string searchTerm)
    {
        var filter = Builders<Medicine>.Filter.Regex(
            "ProductName",
            new BsonRegularExpression(searchTerm, "i"));
        var sort = Builders<Medicine>.Sort.Ascending(m => m.ProductName);
        return await Collection.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<IEnumerable<string>> GetAllSubCategoriesAsync()
    {
        var filter = Builders<Medicine>.Filter.Empty;
        var categories = await Collection.Distinct<string>("SubCategory", filter).ToListAsync();
        return categories;
    }

    public async Task BulkInsertAsync(IEnumerable<Medicine> medicines)
    {
        var medicineList = medicines.ToList();
        if (medicineList.Count == 0) return;

        try
        {
            await Collection.InsertManyAsync(medicineList, new InsertManyOptions { IsOrdered = false });
        }
        catch (MongoBulkWriteException ex)
        {
            Console.WriteLine($"Bulk insert partial failure: {ex.WriteErrors.Count} errors");
        }
    }
}
