using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingRepository"/>.
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
    /// Adds a building to the database and returns the persisted entity.
    /// Implicitly satisfies <see cref="IBuildingRepository.AddAsync"/> because
    /// <see cref="Task{Building}"/> derives from <see cref="Task"/>.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The persisted building.</returns>
    public Task<Building> AddAsync(Building building)
    {
        _dbContext.Buildings.Add(building);
        _dbContext.SaveChanges();
        return Task.FromResult(building);
    }

    /// <summary>
    /// Checks whether a building with the given name exists.
    /// </summary>
    /// <param name="name">The building name.</param>
    /// <returns>True if it exists; otherwise false.</returns>
    public Task<bool> ExistsByNameAsync(string name)
    {
        return Task.FromResult(_dbContext.Buildings.Any(b => b.Name == name));
    }
}
