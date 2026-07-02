using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps domain entities to presentation DTOs and responses for learning components.
/// </summary>
internal static class LearningComponentMapper
{
    /// <summary>
    /// Maps a collection of LearningComponent entities to a GetLearningComponentsResponse.
    /// </summary>
    /// <param name="components">The domain entities to map.</param>
    /// <returns>A response containing the mapped DTOs.</returns>
    internal static GetLearningComponentsResponse ToResponse(IEnumerable<LearningComponent> components)
    {
        var dtos = components.Select(ToDto).ToList();
        return new GetLearningComponentsResponse(dtos);
    }

    /// <summary>
    /// Maps a LearningComponent entity to a LearningComponentDto.
    /// </summary>
    /// <param name="component">The domain entity to map.</param>
    /// <returns>The mapped DTO.</returns>
    internal static LearningComponentDto ToDto(LearningComponent component)
    {
        return new LearningComponentDto(
            component.ComponentId,
            component.LearningSpaceId,
            component.Width,
            component.Height,
            component.Depth,
            component.X,
            component.Y,
            component.Z,
            component.Orientation);
    }

    /// <summary>
    /// Creates an error response for invalid learning space ID.
    /// </summary>
    /// <returns>An error response with appropriate message.</returns>
    internal static ErrorResponse InvalidLearningSpaceIdError()
    {
        return new ErrorResponse("Learning space ID cannot be null or empty");
    }

    /// <summary>
    /// Creates an error response for not found scenarios.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An error response with the specified message.</returns>
    internal static ErrorResponse NotFoundError(string message)
    {
        return new ErrorResponse(message);
    }
}