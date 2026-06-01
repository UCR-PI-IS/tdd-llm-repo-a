using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Encapsulates the database query logic for learning components.
/// </summary>
internal static class LearningComponentQuery
{
    /// <summary>
    /// Retrieves all learning components for a given learning space from the database context.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>A list of matching learning components.</returns>
    public static List<LearningComponent> GetByLearningSpaceId(
        UCRDatabaseContext dbContext,
        string learningSpaceId)
    {
        return dbContext.LearningComponents
            .Where(c => c.LearningSpaceId == learningSpaceId)
            .ToList();
    }
}