using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.ComponentsManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.Spaces;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.Spaces;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.ComponentsManagement;

/// <summary>
/// SQL implementation of the <see cref="ILearningComponentRepository"/> interface.
/// </summary>
internal class SqlLearningComponentRepository : ILearningComponentRepository
{
    /// <summary>
    /// The database context used to interact with the database.
    /// </summary>
    private readonly ThemeParkDataBaseContext _dbContext;

    /// <summary>
    /// Object for display logging information and errors.
    /// </summary>
    private readonly ILogger<SqlLearningComponentRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningComponentRepository"/> class.
    /// </summary>
    /// <param name="dbContext"></param>
    public SqlLearningComponentRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlLearningComponentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all learning components in the system.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<LearningComponent>> GetAllAsync()
    {
        return await _dbContext.LearningComponents.ToListAsync();
    }

    public async Task<LearningComponent> GetSingleLearningComponentAsync(int id)
    {
        Id componentId = Id.Create(id);
        LearningComponent? component = null;

        try
        {
            component = await _dbContext
                .LearningComponents
                .FirstOrDefaultAsync(c => c.ComponentId == componentId.ValueInt);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving component with '{Id}': {Message}",
                id, 
                ex.Message);
        }

        if (component is null)
        {
            string message = $"Component with ID '{id}' not found.";
            throw new NotFoundException(message);
        }
        
        return component;
    }

    /// <summary>
    /// Retrieves learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId"></param>
    /// <returns></returns>
    public async Task<IEnumerable<LearningComponent>> GetLearningComponentsByIdAsync(int learningSpaceId)
    {
        try
        {
            // Validate that the learning space exists and belongs to the floor
            bool learningSpaceExists = await _dbContext.LearningSpaces
                .AnyAsync(ls =>
                    EF.Property<int>(ls, "LearningSpaceId") == learningSpaceId);

            if (!learningSpaceExists)
            {
                _logger.LogWarning("Cannot add component: Learning space with ID {LearningSpaceId}", learningSpaceId);
                return Enumerable.Empty<LearningComponent>();
            }
            // Retrieve learning components that belong to the specified learning space
            var components = await _dbContext.LearningComponents
                .Where(lc => EF.Property<int>(lc, "LearningSpaceId") == learningSpaceId)
                .ToListAsync();

            return components;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error retrieving components for learning space '{LearningSpaceId}'': {Message}",
                learningSpaceId, 
                ex.Message);

            return Enumerable.Empty<LearningComponent>();
        }
    }



    /// <summary>
    /// Adds a new learning component to the database.
    /// </summary>
    /// <param name="learningComponent"></param>
    /// <returns></returns>
    public async Task<bool> AddComponentAsync(int learningSpaceId, LearningComponent learningComponent)
    {

        // Validate that the learning space exists and belongs to the floor
        bool learningSpaceExists = await _dbContext.LearningSpaces
            .AnyAsync(ls =>
                EF.Property<int>(ls, "LearningSpaceId") == learningSpaceId);

        if (!learningSpaceExists)
        {
            _logger.LogWarning("Cannot add component: Learning space with ID {LearningSpaceId} not found on Floor", learningSpaceId);
            return false;
        }
        try
        {
            // Add the LearningComponent to the context
            await _dbContext.LearningComponents.AddAsync(learningComponent);

            // Set shadow properties
            _dbContext.Entry(learningComponent).Property("LearningSpaceId").CurrentValue = learningSpaceId;

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Component added successfully to Learning Space ID {LearningSpaceId}", learningSpaceId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Error adding component to Learning Space ID {LearningSpaceId}: {Message}",
                learningSpaceId,
                ex.Message);
            return false;
        }
    }


    public async Task<bool> UpdateAsync(int learningSpaceId, int learningComponentId, LearningComponent learningComponent)
    {
        try
        {
            // Validate that the learning space exists and belongs to the floor
            bool learningSpaceExists = await _dbContext.LearningSpaces
                .AnyAsync(ls =>
                    EF.Property<int>(ls, "LearningSpaceId") == learningSpaceId);

            if (!learningSpaceExists)
            {
                _logger.LogWarning(
                    "Cannot add component: Learning space with ID {LearningSpaceId}",
                    learningSpaceId);
                return false;
            }

            // Find the existing learning component using only the component ID and learning space ID
            var existing = await _dbContext.LearningComponents.FirstOrDefaultAsync(lc =>
                lc.ComponentId == learningComponentId && EF.Property<int>(lc, "LearningSpaceId") == learningSpaceId);

            if (existing is null)
            {
                _logger.LogWarning(
                    "Cannot update: Component ID {ComponentId} not found in learning space '{LearningSpaceId}'.",
                    learningComponent.ComponentId, 
                    learningSpaceId);
                return false;
            }

            // Detach existing entity and set the shadow property
            learningComponent.ComponentId = existing.ComponentId;
            _dbContext.Entry(existing).State = EntityState.Detached;
            _dbContext.Entry(learningComponent).Property("LearningSpaceId").CurrentValue = learningSpaceId;

            _dbContext.Update(learningComponent);
            int result = await _dbContext.SaveChangesAsync();

            _logger.LogInformation(
                "Component ID {ComponentId} successfully updated in learning space '{LearningSpaceId}'.",
                existing.ComponentId, 
                learningSpaceId);

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error updating component ID {ComponentId} in learning space '{LearningSpaceId}': {Message}", 
                learningComponent.ComponentId,
                learningSpaceId, 
                ex.Message);
            return false;
        }
    }



    /// <summary>
    ///  Deletes a learning component from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteComponentAsync(int id)
    {
        Id componentId = Id.Create(id);

        var component = await _dbContext.LearningComponents
            .FirstOrDefaultAsync(c => c.ComponentId == componentId.ValueInt);

        if (component is null)
            return false;

        _dbContext.LearningComponents.Remove(component);
        int r = await _dbContext.SaveChangesAsync();
        return r > 0;
    }
}
