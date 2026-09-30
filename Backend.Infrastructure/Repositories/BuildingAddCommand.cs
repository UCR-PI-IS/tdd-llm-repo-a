using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Helper for adding a building entity and persisting it.
/// </summary>
internal static class BuildingAddCommand
{
    public static async Task<Building> AddAsync(UCRDatabaseContext context, Building building)
    {
        context.Buildings.Add(building);
        await context.SaveChangesAsync();
        return building;
    }
}
