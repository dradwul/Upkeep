namespace Upkeep.Models;

public sealed class SubCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string? GroupName { get; set; }

    public bool? IsSampleData { get; set; }

    public List<LogEntry> LogEntries { get; set; } = [];
}
