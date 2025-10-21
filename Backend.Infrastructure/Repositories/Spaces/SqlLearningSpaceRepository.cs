using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.Spaces;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.Spaces;

/// <summary>
/// SQL Server implementation of the <see cref="ILearningSpaceRepository"/> interface.
/// This class handles data access for learning spaces using Entity Framework Core.
/// </summary>
internal class SqlLearningSpaceRepository : ILearningSpaceRepository
{
    private readonly ThemeParkDataBaseContext _dbContext;

    /// <summary>
    /// Object for logging information and errors.
    /// </summary>
    private readonly ILogger<SqlLearningSpaceRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningSpaceRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used to access the theme park database.</param>
    /// <param name="logger">The logger instance for logging operations.</param>
    public SqlLearningSpaceRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlLearningSpaceRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Adds a new learning space to the database.
    /// </summary>
    /// <param name="floorId"> The internal Id of the floor where the learning space will be created.</param>
    /// <param name="learningSpace"> The <see cref="LearningSpace"/> entity to be created.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result is true if the creation was successful; otherwise, false.
    /// </returns>
    public async Task<bool> CreateLearningSpaceAsync(int floorId, LearningSpace learningSpace)
    {
        var message = string.Empty;

        var floorExists = await _dbContext.Floors.AnyAsync(f => f.FloorId == floorId);
        if (!floorExists)
        {
            message = $"Floor with Id '{floorId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        try
        {
            // Set necessary shadow 
            _dbContext.Entry(learningSpace).Property("FloorId").CurrentValue = floorId;

            // Add the learning space
            await _dbContext.LearningSpaces.AddAsync(learningSpace);

            // Attempt to save changes
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Learning space created successfully in floor {floorId}.", floorId);
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            message = $"Concurrency conflict creating the learning space '{learningSpace.Name.Name}' on floor '{floorId}'.";
            _logger.LogWarning(ex, message);
            throw new ConcurrencyConflictException(message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            message = $"Unable to create learning space '{learningSpace.Name.Name}'. The specified floor ID '{floorId}' does not exist.";
            _logger.LogWarning(ex, message);
            throw new NotFoundException(message); 
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            message = $"A learning space with the name '{learningSpace.Name.Name}' already exists on this floor.";
            _logger.LogWarning(ex, message);
            throw new DuplicatedEntityException(message);
        }
    }

    /// <summary>
    /// Updates an existing learning space identified by its id.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space to be updated.</param>
    /// <param name="learningSpace">The updated learning space data.</param>
    /// <returns>True if updated successfully, false otherwise.</returns>
    public async Task<bool> UpdateLearningSpaceAsync(int learningSpaceId, LearningSpace learningSpace)
    {
        var message = string.Empty;
        var space = await _dbContext.LearningSpaces.FirstOrDefaultAsync(ls => ls.LearningSpaceId == learningSpaceId);

        if (space is null)
        {
            message = $"Learning space with the Id '{learningSpaceId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        try
        {
            space.Name = learningSpace.Name;
            space.Type = learningSpace.Type;
            space.MaxCapacity = learningSpace.MaxCapacity;
            space.Height = learningSpace.Height;
            space.Width = learningSpace.Width;
            space.Length = learningSpace.Length;

            // Attempt to update the learning space
            await _dbContext.SaveChangesAsync();

            message = $"Learning space '{space.Name.Name}' (Id: {learningSpaceId}) successfully updated.";
            _logger.LogInformation(message);
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            message = $"Concurrency conflict updating learning space with Id '{learningSpaceId}'. It may have been updated or deleted by another user.";
            _logger.LogWarning(ex, message);
            throw new ConcurrencyConflictException(message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            message = $"A learning space with the name '{learningSpace.Name.Name}' already exists on this floor.";
            _logger.LogWarning(ex, message);
            throw new DuplicatedEntityException(message);
        }
    }

    /// <summary>
    /// Retrieves a learning space by its identifier on a specific floor of a building.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// The matching learning space if found; otherwise, null.
    /// </returns>
    public async Task<LearningSpace?> ReadLearningSpaceAsync(int learningSpaceId)
    {
        var learningSpace = await _dbContext.LearningSpaces.FirstOrDefaultAsync(ls => ls.LearningSpaceId == learningSpaceId);

        if (learningSpace is null)
        {
            var message = $"Learning Space with id '{learningSpaceId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        return learningSpace;
    }

    /// <summary>
    /// Deletes a learning space by its identifier.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space to be deleted.</param>
    /// <returns>True if the learning space was successfully deleted; otherwise, throws a NotFoundException.</returns>
    public async Task<bool> DeleteLearningSpaceAsync(int learningSpaceId)
    {
        var message = string.Empty;

        // Search for the learning space that belongs to the specified floor
        var learningSpace = await _dbContext.LearningSpaces
            .FirstOrDefaultAsync(ls => ls.LearningSpaceId == learningSpaceId);

        if (learningSpace is null)
        {
            message = $"Learning space with the Id '{learningSpaceId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        try
        {
            // Attempt to delete the learning space
            _dbContext.LearningSpaces.Remove(learningSpace);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            message = $"Concurrency conflict deleting learning space with the Id '{learningSpaceId}'. It may have been updated or deleted by another user.";
            _logger.LogWarning(ex, message);
            throw new ConcurrencyConflictException(message);
        }
    }

    /// <summary>
    /// Retrieves a list of all learning spaces on a specific floor in a building.
    /// </summary>
    /// <param name="floorId">The identifier of the floor within the building.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a list of <see cref="LearningSpace"/> instances on the specified floor of a building.
    /// ; otherwise, null.
    /// </returns>
    public async Task<List<LearningSpace>?> ListLearningSpacesAsync(int floorId)
    {
        var message = string.Empty;

        try
        {
            var learningSpaces = await _dbContext.LearningSpaces
                .Where(ls => EF.Property<int>(ls, "FloorId") == floorId)
                .ToListAsync();

            if (learningSpaces == null || !learningSpaces.Any())
            {
                message = $"No learning spaces found for floorId {floorId}.";
                _logger.LogInformation(message);
                throw new NotFoundException(message);
            }

            return learningSpaces;
        }
        catch (DbUpdateException ex)
        {
            message = $"Database update error while listing learning spaces for floorId {floorId}.";
            _logger.LogWarning(ex, message);
            throw new DbUpdateException(message);
        }

    }
}