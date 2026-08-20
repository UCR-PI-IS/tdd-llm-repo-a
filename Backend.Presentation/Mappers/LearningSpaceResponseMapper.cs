using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps learning space domain entities to presentation response objects.
/// </summary>
internal static class LearningSpaceResponseMapper
{
    /// <summary>
    /// Converts a <see cref="LearningSpace"/> entity to a <see cref="LearningSpaceResponse"/>.
    /// </summary>
    /// <param name="learningSpace">The domain entity to convert.</param>
    /// <returns>The corresponding response object.</returns>
    public static LearningSpaceResponse ToResponse(LearningSpace learningSpace) =>
        new(learningSpace.LearningSpaceId, learningSpace.Type, learningSpace.Height, learningSpace.Width, learningSpace.Length);
}
