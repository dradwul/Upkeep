namespace Upkeep.Models;

public sealed class LogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public string Title { get; set; } = string.Empty;

    public bool? IsSampleData { get; set; }

    public string Notes { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public string? Specifications { get; set; }

    public decimal? Cost { get; set; }

    public int? Mileage { get; set; }
}
