using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
/// Provides building data access operations.
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
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    public async Task<Building> AddAsync(Building building)
    {
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();
        return building;
    }

    /// <summary>
    /// Checks if a building with the specified name already exists in the database.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists; otherwise, false.</returns>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbContext.Buildings.AnyAsync(b => b.Name == name);
    }

    /// <summary>
    /// Checks if an area with the specified ID exists.
    /// For now, returns true as area validation is not yet implemented.
    /// </summary>
    /// <param name="areaId">The area ID to check.</param>
    /// <returns>True if the area exists; otherwise, false.</returns>
    public Task<bool> AreaExistsAsync(int areaId)
    {
        // For now, assume all areas exist since we don't have an Area entity yet
        return Task.FromResult(true);
    }
}
