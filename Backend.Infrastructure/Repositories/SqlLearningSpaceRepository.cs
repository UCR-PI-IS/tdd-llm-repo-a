using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningSpaceRepository"/>.
/// Provides creation operations for learning space data in the database.
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
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(LearningSpace learningSpace)
    {
        _dbContext.LearningSpaces.Add(learningSpace);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets a learning space by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the learning space.</param>
    /// <returns>The learning space entity, or null if not found.</returns>
    public async Task<LearningSpace?> GetByIdAsync(string id)
    {
        // Try to parse the ID as an integer
        if (int.TryParse(id, out int learningSpaceId))
        {
            return await _dbContext.LearningSpaces
                .FirstOrDefaultAsync(ls => ls.LearningSpaceId == learningSpaceId);
        }

        // If not a valid integer, return null
        return null;
    }
}
