namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new learning component.
/// </summary>
/// <param name="ComponentId">Optional unique identifier. If null, an ID will be auto-generated.</param>
/// <param name="LearningSpaceId">Identifier of the learning space.</param>
/// <param name="Width">Width of the component in meters.</param>
/// <param name="Height">Height of the component in meters.</param>
/// <param name="Depth">Depth of the component in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the component.</param>
public record class CreateLearningComponentDto(
    string? ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation);
