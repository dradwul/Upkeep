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
            return JsonSerializer.Deserialize<List<Category>>(storedData, JsonOptions)
                ?? throw new InvalidDataException("Upkeep data in local storage could not be read.");
        }

        var sampleCategories = CreateSampleData();
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
                    Name = "Service",
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 10, 12),
                            Title = "Byte till vinterdäck",
                            Notes = "Däcken monterade och balanserade inför vintern.",
                            Brand = "Michelin",
                            Specifications = "205/55 R16"
                        },
                        new LogEntry
                        {
                            Date = new DateOnly(2025, 6, 3),
                            Title = "Nya bromsskivor fram",
                            Notes = "Bromsskivor och belägg bytta på båda framhjulen.",
                            Brand = "Bosch",
                            Cost = 4200m,
                            Mileage = 82450
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
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2024, 3, 18),
                            Title = "Ny bänkskiva",
                            Notes = "Ek, matt hårdvaxolja.",
                            Brand = "IKEA",
                            Model = "Karlby"
                        }
                    ]
                },
                new SubCategory
                {
                    Name = "Tak",
                    LogEntries =
                    [
                        new LogEntry
                        {
                            Date = new DateOnly(2023, 8, 2),
                            Title = "Besiktning av yttertak",
                            Notes = "Taket är i gott skick. Följ upp efter nästa vinter.",
                            Specifications = "Betongpannor"
                        }
                    ]
                }
            ]
        }
    ];
}
