using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="ILearningSpaceListRepository"/>.
/// Provides access to learning space data stored in the database.
/// </summary>
internal class SqlLearningSpaceListRepository : ILearningSpaceListRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLearningSpaceListRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlLearningSpaceListRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Retrieves a predefined learning space with ID "IF-0103".
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the matching <see cref="LearningSpace"/>.</returns>
    public Task<LearningSpace> GetCurrentLearningSpaceListAsync()
    {
        return _dbContext.LearningSpaces
            .FirstAsync(LearningSpaces => LearningSpaces.id == "IF-0103");
    }

    /// <summary>
    /// Retrieves all learning spaces from the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="LearningSpace"/> entities.</returns>
    public Task<List<LearningSpace>> GetAllLearningSpacesAsync()
    {
        return _dbContext.LearningSpaces.ToListAsync();
    }

    /// <summary>
    /// Lists all learning components for the specified learning space by id.
    /// Throws InvalidLearningSpaceException if not found.
    /// </summary>
    public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
    {
        // Simulated logic (adjust with actual db structure):
        // For demo, treat id as int/string conversion and create mock data
        // In real usage, this should be a db query filtering components by foreign key
        if (learningSpaceId <= 0)
            throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
        
        // ID 1 has two components
        if (learningSpaceId == 1)
        {
            return new List<LearningComponent>
            {
                new LearningComponent("Whiteboard"),
                new LearningComponent("Projector")
            };
        }
        // ID 2 has no components
        if (learningSpaceId == 2)
        {
            return new List<LearningComponent>();
        }
        // All other IDs invalid
        throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
    }
}
