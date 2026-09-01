namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a whiteboard.
/// </summary>
/// <param name="ComponentId">Unique identifier for the whiteboard.</param>
/// <param name="LearningSpaceId">Identifier of the learning space this whiteboard belongs to.</param>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the whiteboard (North, South, East, or West).</param>
/// <param name="MarkerColor">Color of the whiteboard markers.</param>
public record CreateWhiteboardDto(
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
