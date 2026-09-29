using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL Server implementation of the building repository.
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

    /// <inheritdoc />
    public Task<Building> AddAsync(Building building)
    {
        _dbContext.Buildings.AddRange(building);
        _dbContext.SaveChanges();
        return Task.FromResult(building);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByNameAsync(string name)
    {
        return _dbContext.Buildings.AnyAsync(b => b.Name == name);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByAreaIdAsync(int areaId)
    {
        return _dbContext.Buildings.AnyAsync(b => b.AreaId == areaId);
    }
}
