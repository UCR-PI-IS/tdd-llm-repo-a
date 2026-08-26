using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningSpaceRepository"/>.
/// Provides data access for learning space creation operations.
/// </summary>
internal class SqlLearningSpaceRepository : ILearningSpaceRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningSpaceRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlLearningSpaceRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new learning space to the database.
    /// </summary>
    /// <param name="learningSpace">The learning space entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(LearningSpace learningSpace)
    {
        // Reset the ID to 0 to let the database generate it
        // The entity sets an in-memory ID for testing, but the database uses IDENTITY
        learningSpace.LearningSpaceId = 0;
        _dbContext.LearningSpaces.Add(learningSpace);
        await _dbContext.SaveChangesAsync();
    }
}
