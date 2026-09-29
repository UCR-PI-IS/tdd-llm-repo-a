using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
/// Provides persistence operations for buildings.
/// </summary>
internal class SqlBuildingRepository : IBuildingRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBuildingRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public SqlBuildingRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The added building.</returns>
    public async Task<Building> AddAsync(Building building)
    {
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();
        return building;
    }

    /// <summary>
    /// Checks whether a building with the specified name exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if the building exists; otherwise, false.</returns>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbContext.Buildings.AnyAsync(b => b.Name == name);
    }
}
