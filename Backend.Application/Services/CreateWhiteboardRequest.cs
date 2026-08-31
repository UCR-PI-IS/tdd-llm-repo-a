namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Represents a request to create a whiteboard.
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
/// <param name="MarkerColor">Marker color of the whiteboard.</param>
public record CreateWhiteboardRequest(
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
