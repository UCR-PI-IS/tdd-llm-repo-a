using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
/// Provides create and query operations for building data in the database.
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
    /// Adds a new building to the database and persists it.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>The added building entity.</returns>
    public Task<Building> AddAsync(Building building)
    {
        return BuildingAddCommand.AddAsync(_dbContext, building);
    }

    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists, otherwise false.</returns>
    public Task<bool> ExistsByNameAsync(string name)
    {
        return BuildingExistsQuery.ExistsByNameAsync(_dbContext, name);
    }
}
