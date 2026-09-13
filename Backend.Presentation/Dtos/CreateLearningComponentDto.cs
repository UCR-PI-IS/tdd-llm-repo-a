namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a learning component.
/// </summary>
/// <param name="componentId">Unique identifier for the component. If null, an ID will be auto-generated.</param>
/// <param name="learningSpaceId">Identifier of the learning space this component belongs to.</param>
/// <param name="width">Width of the component in meters.</param>
/// <param name="height">Height of the component in meters.</param>
/// <param name="depth">Depth of the component in meters.</param>
/// <param name="x">X coordinate position.</param>
/// <param name="y">Y coordinate position.</param>
/// <param name="z">Z coordinate position.</param>
/// <param name="orientation">Orientation of the component.</param>
public record class CreateLearningComponentDto(
    string? componentId,
    string learningSpaceId,
    float width,
    float height,
    float depth,
    float x,
    float y,
    float z,
    string orientation);
