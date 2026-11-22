using Upkeep.Models;

namespace Upkeep.Services;

public interface ILogService
{
    Task<IReadOnlyList<Category>> GetCategoriesAsync();

    Task SaveCategoriesAsync(IEnumerable<Category> categories);
}
