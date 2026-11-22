using System.Text.Json;
using Microsoft.JSInterop;
using Upkeep.Models;

namespace Upkeep.Services;

public sealed class LocalStorageLogService(IJSRuntime jsRuntime) : ILogService
{
    private const string StorageKey = "upkeep-data";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync()
    {
        var storedData = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
        if (storedData is not null)
        {
            var categories = JsonSerializer.Deserialize<List<Category>>(storedData, JsonOptions)
                ?? throw new InvalidDataException("Upkeep data in local storage could not be read.");
            if (UpgradeSampleDataMarkers(categories))
            {
                await SaveCategoriesAsync(categories);
            }
            return categories;
        }

        var sampleCategories = CreateSampleData();
        UpgradeSampleDataMarkers(sampleCategories);
        await SaveCategoriesAsync(sampleCategories);
        return sampleCategories;
    }

    public async Task SaveCategoriesAsync(IEnumerable<Category> categories)
    {
        var serializedData = JsonSerializer.Serialize(categories, JsonOptions);
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, serializedData);
    }

    private static List<Category> CreateSampleData() =>
    [
        new Category
        {
            Name = "Bilen",
            Icon = "car-front",
            IsSampleData = true,
            SubCategories =
            [
                new SubCategory
                {
                    Name = "Oljebyte",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 5, 19),
                            Title = "Oljebyte",
                            Notes = "Motorolja och filter bytta.",
                            Brand = "Castrol",
                            Specifications = "0W-30",
                            Mileage = 80000,
                            IsSampleData = true
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Service",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 10, 12),
                            Title = "Årlig service",
                            Notes = "Kontroll av bilens viktigaste funktioner.",
                            Brand = "Volvo",
                            Specifications = "Nästa service vid 90 000 km",
                            IsSampleData = true
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Däck",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 10, 12),
                            Title = "Byte till vinterdäck",
                            Notes = "Däcken monterade och balanserade inför vintern.",
                            Brand = "Michelin",
                            Specifications = "205/55 R16",
                            IsSampleData = true
                        },
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 4, 8),
                            Title = "Byte till sommardäck",
                            Notes = "Sommardäcken monterade.",
                            Brand = "Michelin",
                            Specifications = "205/55 R16",
                            IsSampleData = true
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Bromsar",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 6, 3),
                            Title = "Nya bromsskivor fram",
                            Notes = "Bromsskivor och belägg bytta på båda framhjulen.",
                            Brand = "Bosch",
                            Cost = 4200m,
                            Mileage = 82450,
                            IsSampleData = true
                        }
                    ]
                }
            ]
        },
        new Category
        {
            Name = "Huset",
            Icon = "house",
            IsSampleData = true,
            SubCategories =
            [
                new SubCategory
                {
                    Name = "Kök",
                    GroupName = "Rum",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2024, 3, 18),
                            Title = "Ny bänkskiva",
                            Notes = "Ek, matt hårdvaxolja.",
                            Brand = "IKEA",
                            Model = "Karlby",
                            IsSampleData = true
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Tak",
                    GroupName = "Ytskikt",
                    IsSampleData = true,
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2023, 8, 2),
                            Title = "Besiktning av yttertak",
                            Notes = "Taket är i gott skick. Följ upp efter nästa vinter.",
                            Specifications = "Betongpannor",
                            IsSampleData = true
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Panel",
                    GroupName = "Ytskikt",
                    IsSampleData = true
                },
                new SubCategory
                {
                    Name = "Fönster",
                    GroupName = "Ytskikt",
                    IsSampleData = true
                }
            ]
        }
    ];

    private static bool UpgradeSampleDataMarkers(IEnumerable<Category> categories)
    {
        var changed = false;
        foreach (var category in categories.Where(category => category.IsSampleData))
        {
            foreach (var subCategory in category.SubCategories.Where(subCategory => subCategory.IsSampleData is null))
            {
                subCategory.IsSampleData = true;
                changed = true;
            }
            foreach (var logEntry in category.SubCategories.SelectMany(subCategory => subCategory.LogEntries)
                .Where(logEntry => logEntry.IsSampleData is null))
            {
                logEntry.IsSampleData = true;
                changed = true;
            }

            if (category.Name.Equals("Bilen", StringComparison.OrdinalIgnoreCase))
            {
                var oilChange = EnsureSampleSubCategory(category, "Oljebyte", null, ref changed);
                var service = EnsureSampleSubCategory(category, "Service", null, ref changed);
                var tires = EnsureSampleSubCategory(category, "Däck", null, ref changed);
                var brakes = EnsureSampleSubCategory(category, "Bromsar", null, ref changed);
                MoveSampleEntry(service, tires, "Byte till vinterdäck", ref changed);
                MoveSampleEntry(service, brakes, "Nya bromsskivor fram", ref changed);
                if (!oilChange.LogEntries.Any(entry => entry.Title.Equals("Oljebyte", StringComparison.OrdinalIgnoreCase)))
                {
                    oilChange.LogEntries.Add(new LogEntry
                    {
                        Date = new DateOnly(2025, 5, 19),
                        Title = "Oljebyte",
                        Notes = "Motorolja och filter bytta.",
                        Brand = "Castrol",
                        Specifications = "0W-30",
                        Mileage = 80000,
                        IsSampleData = true
                    });
                    changed = true;
                }
            }
            else if (category.Name.Equals("Huset", StringComparison.OrdinalIgnoreCase) ||
                     category.Name.Equals("Hemmet", StringComparison.OrdinalIgnoreCase))
            {
                EnsureSampleSubCategory(category, "Tak", "Ytskikt", ref changed);
                EnsureSampleSubCategory(category, "Panel", "Ytskikt", ref changed);
                EnsureSampleSubCategory(category, "Fönster", "Ytskikt", ref changed);
                EnsureSampleSubCategory(category, "Dörrar", "Ytskikt", ref changed);
                EnsureSampleSubCategory(category, "Kök", "Rum", ref changed);
            }
        }

        return changed;
    }

    private static SubCategory EnsureSampleSubCategory(
        Category category,
        string name,
        string? groupName,
        ref bool changed)
    {
        var subCategory = category.SubCategories.FirstOrDefault(subCategory =>
            subCategory.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (subCategory is null)
        {
            subCategory = new SubCategory { Name = name, GroupName = groupName, IsSampleData = true };
            category.SubCategories.Add(subCategory);
            changed = true;
        }
        else
        {
            if (subCategory.IsSampleData is null)
            {
                subCategory.IsSampleData = true;
                changed = true;
            }
            if (groupName is not null && subCategory.GroupName != groupName)
            {
                subCategory.GroupName = groupName;
                changed = true;
            }
        }

        return subCategory;
    }

    private static void MoveSampleEntry(
        SubCategory source,
        SubCategory destination,
        string title,
        ref bool changed)
    {
        var entry = source.LogEntries.FirstOrDefault(entry =>
            entry.IsSampleData == true && entry.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (entry is null || destination.LogEntries.Any(destinationEntry => destinationEntry.Id == entry.Id))
        {
            return;
        }
        source.LogEntries.Remove(entry);
        destination.LogEntries.Add(entry);
        changed = true;
    }
}
