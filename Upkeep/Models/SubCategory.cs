namespace Upkeep.Models;

public sealed class SubCategory
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public List<LogEntry> LogEntries { get; set; } = [];
}
