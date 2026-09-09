using Microsoft.EntityFrameworkCore;
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
    /// Retrieves all learning components that belong to the specified learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components belonging to the specified learning space.</returns>
    public async Task<List<LearningComponent>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        return await _dbContext.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToListAsync();
    }

    /// <summary>
    /// Checks whether a component with the specified ID already exists in the database.
    /// </summary>
    /// <param name="componentId">The component identifier to check.</param>
    /// <returns>True if a component with the given ID exists; otherwise, false.</returns>
    public async Task<bool> ExistsAsync(string componentId)
    {
        var matching = await _dbContext.LearningComponents
            .Where(c => c.ComponentId == componentId)
            .ToListAsync();
        return matching.Count > 0;
    }

    /// <summary>
    /// Adds a new learning component to the database and saves changes.
    /// </summary>
    /// <param name="component">The learning component to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(LearningComponent component)
    {
        await _dbContext.LearningComponents.AddAsync(component);
        await _dbContext.SaveChangesAsync();
    }
}
