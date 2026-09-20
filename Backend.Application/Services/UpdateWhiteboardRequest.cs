namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object carrying the parameters needed to update a whiteboard.
/// </summary>
/// <param name="ComponentId">Unique identifier for the whiteboard to update.</param>
/// <param name="Width">New width of the whiteboard in meters.</param>
/// <param name="Height">New height of the whiteboard in meters.</param>
/// <param name="Depth">New depth of the whiteboard in meters.</param>
/// <param name="X">New X coordinate position.</param>
/// <param name="Y">New Y coordinate position.</param>
/// <param name="Z">New Z coordinate position.</param>
/// <param name="Orientation">New orientation of the whiteboard.</param>
/// <param name="MarkerColor">New color of the whiteboard marker.</param>
public record class UpdateWhiteboardRequest(
    string ComponentId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
