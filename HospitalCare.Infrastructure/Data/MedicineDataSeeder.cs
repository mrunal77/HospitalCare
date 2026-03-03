using HospitalCare.Domain.Entities;
using HospitalCare.Infrastructure.Data.MongoDB;
using MongoDB.Driver;

namespace HospitalCare.Infrastructure.Data;

public static class MedicineDataSeeder
{
    private const int BatchSize = 1000;
    private const string CsvPath = "archive/medicine_data.csv";

    public static async Task SeedMedicinesAsync(MongoDbContext context)
    {
        var medicineCount = await context.Medicines.CountDocumentsAsync(_ => true);
        if (medicineCount > 0)
        {
            Console.WriteLine($"Medicines already seeded ({medicineCount} records found). Skipping...");
            return;
        }

        var csvFullPath = Path.Combine(AppContext.BaseDirectory, CsvPath);
        if (!File.Exists(csvFullPath))
        {
            csvFullPath = Path.Combine(Directory.GetCurrentDirectory(), CsvPath);
        }
        if (!File.Exists(csvFullPath))
        {
            csvFullPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName ?? "", CsvPath);
        }
        if (!File.Exists(csvFullPath))
        {
            csvFullPath = "/home/mrunal/Projects/HospitalCare/archive/medicine_data.csv";
        }

        if (!File.Exists(csvFullPath))
        {
            Console.WriteLine($"Medicine CSV file not found at: {csvFullPath}");
            return;
        }

        Console.WriteLine($"Starting medicine data import from: {csvFullPath}");

        var lines = await File.ReadAllLinesAsync(csvFullPath);
        var totalRecords = lines.Length - 1;
        Console.WriteLine($"Found {totalRecords} medicine records to import");

        var batch = new List<Medicine>(BatchSize);
        var importedCount = 0;
        var startTime = DateTime.Now;

        for (int i = 1; i < lines.Length; i++)
        {
            try
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var medicine = ParseCsvLine(line);
                if (medicine != null)
                {
                    batch.Add(medicine);
                }

                if (batch.Count >= BatchSize)
                {
                    await InsertBatchAsync(context, batch);
                    importedCount += batch.Count;
                    Console.WriteLine($"Imported {importedCount}/{totalRecords} medicines...");
                    batch.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing line {i}: {ex.Message}");
            }
        }

        if (batch.Count > 0)
        {
            await InsertBatchAsync(context, batch);
            importedCount += batch.Count;
        }

        var elapsed = DateTime.Now - startTime;
        Console.WriteLine($"Completed importing {importedCount} medicines in {elapsed.TotalSeconds:F2} seconds");
    }

    private static Medicine? ParseCsvLine(string line)
    {
        var values = ParseCsvFields(line);
        if (values.Length < 6) return null;

        try
        {
            var subCategory = CleanValue(values[0]);
            var productName = CleanValue(values[1]);
            var saltComposition = CleanValue(values[2]);
            var priceStr = CleanValue(values[3]).Replace("₹", "").Replace(",", "").Trim();
            decimal? price = null;
            if (decimal.TryParse(priceStr, out var parsedPrice))
            {
                price = parsedPrice;
            }
            var manufacturer = CleanValue(values[4]);
            var description = CleanValue(values[5]);
            var sideEffects = values.Length > 6 ? CleanValue(values[6]) : null;
            var drugInteractions = values.Length > 7 ? CleanValue(values[7]) : null;

            if (string.IsNullOrWhiteSpace(productName)) return null;

            return new Medicine(
                subCategory,
                productName,
                saltComposition,
                price,
                manufacturer,
                description,
                sideEffects,
                drugInteractions
            );
        }
        catch
        {
            return null;
        }
    }

    private static string CleanValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        return value.Trim();
    }

    private static string[] ParseCsvFields(string line)
    {
        var fields = new List<string>();
        var currentField = new System.Text.StringBuilder();
        var inQuotes = false;

        foreach (var c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }
        fields.Add(currentField.ToString());

        return fields.ToArray();
    }

    private static async Task InsertBatchAsync(MongoDbContext context, List<Medicine> batch)
    {
        try
        {
            await context.Medicines.InsertManyAsync(batch, new InsertManyOptions { IsOrdered = false });
        }
        catch (MongoBulkWriteException ex)
        {
            Console.WriteLine($"Batch insert error: {ex.WriteErrors.Count} failures");
        }
    }
}
