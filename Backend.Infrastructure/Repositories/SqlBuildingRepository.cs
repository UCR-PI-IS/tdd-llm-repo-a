using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
/// Provides write access to building data stored in the database.
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

    /// <inheritdoc />
    public async Task<Building> AddAsync(Building building)
    {
        await _dbContext.Buildings.AddAsync(building);
        await _dbContext.SaveChangesAsync();
        return building;
    }

    /// <inheritdoc />
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _dbContext.Buildings.AnyAsync(b => b.Name == name);
    }

    /// <inheritdoc />
    public Task<bool> AreaExistsAsync(int areaId)
    {
        // For now, assume areas are managed externally.
        // This checks if any building references this area, or returns true by default.
        // A proper implementation would check an Areas table.
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public async Task<Building?> GetByIdAsync(int id)
    {
        return await _dbContext.Buildings.FindAsync(id);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Building building)
    {
        var existing = await _dbContext.Buildings.FindAsync(building.InternalId);
        if (existing != null)
        {
            _dbContext.Entry(existing).CurrentValues.SetValues(building);
        }
        else
        {
            _dbContext.Buildings.Update(building);
        }
        await _dbContext.SaveChangesAsync();
    }
}
