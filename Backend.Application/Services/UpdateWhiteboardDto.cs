namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating a whiteboard.
/// </summary>
/// <param name="WhiteboardId">The identifier of the whiteboard to update.</param>
/// <param name="Width">New width of the whiteboard in meters.</param>
/// <param name="Height">New height of the whiteboard in meters.</param>
/// <param name="Depth">New depth of the whiteboard in meters.</param>
/// <param name="X">New X coordinate position.</param>
/// <param name="Y">New Y coordinate position.</param>
/// <param name="Z">New Z coordinate position.</param>
/// <param name="Orientation">New orientation of the whiteboard.</param>
/// <param name="MarkerColor">New marker color of the whiteboard.</param>
public record class UpdateWhiteboardDto(
    string WhiteboardId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
