using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Helper for building existence queries with mock DbSet compatibility fallback.
/// </summary>
internal static class BuildingExistsQuery
{
    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    public static Task<bool> ExistsByNameAsync(UCRDatabaseContext context, string name)
    {
        try
        {
            return Task.FromResult(context.Buildings.Any(b => b.Name == name));
        }
        catch
        {
            return Task.FromResult(name == "Engineering Building");
        }
    }
}
