using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningSpaceRepository"/>.
/// Provides persistence for learning space entities.
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
    /// Adds a new learning space to the database and persists it.
    /// </summary>
    /// <param name="learningSpace">The learning space entity to add.</param>
    public async Task AddAsync(LearningSpace learningSpace)
    {
        _dbContext.LearningSpaces.Add(learningSpace);
        await _dbContext.SaveChangesAsync();
    }
}
