using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IWhiteboardRepository"/>.
/// Provides creation, read, and update operations for whiteboard data in the database.
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
    /// <exception cref="DatabaseException">Thrown when the database save operation fails.</exception>
    public async Task AddAsync(Whiteboard whiteboard)
    {
        try
        {
            await _dbContext.Whiteboards.AddAsync(whiteboard);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new DatabaseException(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a whiteboard by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the whiteboard.</param>
    /// <returns>The whiteboard if found; otherwise, null.</returns>
    public async Task<Whiteboard?> GetByIdAsync(string id)
    {
        return await _dbContext.Whiteboards.FindAsync(id);
    }

    /// <summary>
    /// Updates an existing whiteboard in the database and persists changes.
    /// </summary>
    /// <param name="whiteboard">The whiteboard entity to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(Whiteboard whiteboard)
    {
        _dbContext.Whiteboards.Update(whiteboard);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all components in a given learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>A list of learning components in the specified learning space.</returns>
    public async Task<List<LearningComponent>> GetByLearningSpaceIdAsync(string learningSpaceId)
    {
        var whiteboards = await _dbContext.Whiteboards
            .Where(w => w.LearningSpaceId == learningSpaceId)
            .ToListAsync();
        return whiteboards.Cast<LearningComponent>().ToList();
    }
}
