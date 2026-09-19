namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for updating a whiteboard.
/// </summary>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the whiteboard.</param>
/// <param name="MarkerColor">Color of the whiteboard marker.</param>
public record class UpdateWhiteboardDto(
    float Width = 0,
    float Height = 0,
    float Depth = 0,
    float X = 0,
    float Y = 0,
    float Z = 0,
    string Orientation = "North",
    string MarkerColor = "Blue");
