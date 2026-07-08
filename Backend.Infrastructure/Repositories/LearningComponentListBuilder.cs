using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Builds a list of domain <see cref="LearningComponent"/> entities from the database context.
/// </summary>
internal static class LearningComponentListBuilder
{
    /// <summary>
    /// Builds a filtered list of learning components for the specified learning space.
    /// </summary>
    public static List<LearningComponent> Build(UCRDatabaseContext dbContext, string learningSpaceId)
    {
        var result = new List<LearningComponent>();
        foreach (var entity in dbContext.LearningComponents)
        {
            if (entity.LearningSpaceId == learningSpaceId)
            {
                result.Add(entity.ToDomain());
            }
        }

        return result;
    }
}
