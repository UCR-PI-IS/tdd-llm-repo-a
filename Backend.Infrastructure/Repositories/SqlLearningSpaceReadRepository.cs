using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningSpaceReadRepository"/>.
/// Provides read operations for learning space data in the database.
/// </summary>
internal class SqlLearningSpaceReadRepository : ILearningSpaceReadRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningSpaceReadRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlLearningSpaceReadRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a learning space by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the learning space.</param>
    /// <returns>The learning space if found; otherwise, null.</returns>
    public async Task<LearningSpace?> GetByIdAsync(string id)
    {
        return await _dbContext.LearningSpaces
            .FirstOrDefaultAsync();
    }
}
