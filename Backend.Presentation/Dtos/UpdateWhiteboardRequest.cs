namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the request data for updating a whiteboard.
/// </summary>
/// <param name="WhiteboardId">The unique identifier of the whiteboard to update.</param>
/// <param name="Width">New width of the whiteboard in meters.</param>
/// <param name="Height">New height of the whiteboard in meters.</param>
/// <param name="Depth">New depth of the whiteboard in meters.</param>
/// <param name="X">New X coordinate position.</param>
/// <param name="Y">New Y coordinate position.</param>
/// <param name="Z">New Z coordinate position.</param>
/// <param name="Orientation">New orientation of the whiteboard.</param>
/// <param name="MarkerColor">New color of the whiteboard marker.</param>
public record UpdateWhiteboardRequest(
    string WhiteboardId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
