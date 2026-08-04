using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningComponentRepository"/>.
/// Provides access to learning component data stored in the database.
/// </summary>
internal class SqlLearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningComponentRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlLearningComponentRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all learning components for a given learning space from the database.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components in the specified learning space.</returns>
    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        return _dbContext.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }
}
