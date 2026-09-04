namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a whiteboard.
/// </summary>
/// <param name="LearningSpaceId">The identifier of the learning space where the whiteboard will be placed.</param>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position within the learning space.</param>
/// <param name="Y">Y coordinate position within the learning space.</param>
/// <param name="Z">Z coordinate position within the learning space.</param>
/// <param name="Orientation">Orientation of the whiteboard (North, South, East, or West).</param>
/// <param name="MarkerColor">Color of the markers for the whiteboard.</param>
public record class CreateWhiteboardDto(
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
