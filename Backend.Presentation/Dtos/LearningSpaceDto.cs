using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning space.
/// </summary>
/// <param name="Id">The unique identifier of the learning space.</param>
/// <param name="Type">The type or category of the learning space.</param>
public record class LearningSpaceDto(string Id, string Type)
{
    /// <summary>
    /// Creates a LearningSpaceDto from a domain entity.
    /// </summary>
    /// <param name="space">The domain entity to map from.</param>
    /// <returns>A new LearningSpaceDto populated with data from the entity.</returns>
    public static LearningSpaceDto FromEntity(LearningSpace space)
    {
        return new LearningSpaceDto(space.id, space.type);
    }
}
