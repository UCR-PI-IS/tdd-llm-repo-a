namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object for creating a whiteboard.
/// </summary>
/// <param name="ComponentId">Unique identifier for the whiteboard.</param>
/// <param name="LearningSpaceId">Identifier of the learning space.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Depth">Depth in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the whiteboard.</param>
/// <param name="MarkerColor">Color of the whiteboard marker.</param>
public record class CreateWhiteboardRequest(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
