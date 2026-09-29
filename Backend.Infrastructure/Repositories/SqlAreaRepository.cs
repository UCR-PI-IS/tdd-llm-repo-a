using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IAreaRepository"/>.
/// </summary>
internal class SqlAreaRepository : IAreaRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlAreaRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public SqlAreaRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Checks whether an area with the given identifier exists.
    /// </summary>
    /// <param name="areaId">The area identifier.</param>
    /// <returns>True if it exists; otherwise false.</returns>
    public Task<bool> ExistsAsync(int areaId)
    {
        return _dbContext.Areas.AnyAsync(a => a.Id == areaId);
    }
}
