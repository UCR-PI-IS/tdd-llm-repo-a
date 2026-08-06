using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

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
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        var query = _dbContext.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId);

        // Check if the provider supports async operations (EF Core provider does)
        if (query.Provider is IAsyncQueryProvider)
        {
            return query.ToListAsync();
        }
        else
        {
            // Fall back to synchronous for mocked providers in unit tests
            return Task.FromResult(query.ToList());
        }
    }
}
