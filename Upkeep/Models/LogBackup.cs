namespace Upkeep.Models;

public sealed class LogBackup
{
    public int FormatVersion { get; set; } = 1;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public List<Category> Categories { get; set; } = [];
}
