namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the learning component.</param>
/// <param name="LearningSpaceId">The identifier of the parent learning space.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Depth">Depth in meters.</param>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
/// <param name="Z">Z coordinate.</param>
/// <param name="Orientation">Orientation (North, South, East, or West).</param>
public record class LearningComponentDto(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation);
