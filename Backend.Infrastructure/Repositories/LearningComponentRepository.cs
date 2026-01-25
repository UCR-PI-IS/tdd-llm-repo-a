using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningComponentRepository"/>.
/// Provides access to learning component data stored in the database.
/// </summary>
internal class LearningComponentRepository : ILearningComponentRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public LearningComponentRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves all learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>A collection of learning components in the specified learning space</returns>
    public async Task<IEnumerable<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId)
    {
        return await _dbContext.LearningComponents
            .Where(component => component.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }
}
