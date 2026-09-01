using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IWhiteboardRepository"/>.
/// Provides creation operations for whiteboard data in the database.
/// </summary>
internal class SqlWhiteboardRepository : IWhiteboardRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlWhiteboardRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlWhiteboardRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new whiteboard to the database and persists it.
    /// </summary>
    /// <param name="whiteboard">The whiteboard entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(Whiteboard whiteboard)
    {
        try
        {
            await _dbContext.Whiteboards.AddAsync(whiteboard);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Database error", ex);
        }
    }
}
