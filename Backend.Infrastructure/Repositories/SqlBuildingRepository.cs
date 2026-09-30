using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
/// Provides add and existence-check operations for buildings.
/// </summary>
internal class SqlBuildingRepository : IBuildingRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBuildingRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlBuildingRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new building to the database and persists the change.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    public async Task<Building> AddAsync(Building building)
    {
        var entry = await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Checks whether a building with the specified name exists in the database.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists; otherwise false.</returns>
    public Task<bool> ExistsByNameAsync(string name)
    {
        return _dbContext.Buildings.AnyAsync(b => b.Name == name);
    }

    /// <summary>
    /// Checks whether an area with the specified identifier exists.
    /// For now, always returns true as areas are not yet modeled separately.
    /// </summary>
    /// <param name="areaId">The area identifier to check.</param>
    /// <returns>True if the area exists; otherwise false.</returns>
    public Task<bool> AreaExistsAsync(int areaId)
    {
        // Areas are not yet a separate entity; accept any positive areaId.
        return Task.FromResult(areaId > 0);
    }
}
