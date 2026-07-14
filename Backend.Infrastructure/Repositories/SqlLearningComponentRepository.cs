using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningComponentRepository"/>.
/// </summary>
public class SqlLearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningComponentRepository"/> class.
    /// </summary>
    public SqlLearningComponentRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all learning components for a given learning space.
    /// </summary>
    public Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        var components = _dbContext.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToList();

        return Task.FromResult(components);
    }
}
