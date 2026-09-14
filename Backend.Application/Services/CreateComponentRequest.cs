namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents a request to create a new learning component.
/// </summary>
/// <param name="ComponentId">Optional explicit component ID. If empty, an ID will be auto-generated.</param>
/// <param name="LearningSpaceId">The identifier of the learning space this component belongs to.</param>
/// <param name="Width">Width of the component in meters.</param>
/// <param name="Height">Height of the component in meters.</param>
/// <param name="Depth">Depth of the component in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the component (North, South, East, or West).</param>
public record class CreateComponentRequest(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation);
