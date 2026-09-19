namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating a whiteboard.
/// </summary>
public class UpdateWhiteboardDto
{
    /// <summary>
    /// Unique identifier for the whiteboard to update.
    /// </summary>
    public string WhiteboardId { get; }

    /// <summary>
    /// New width of the whiteboard in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// New height of the whiteboard in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// New depth of the whiteboard in meters.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// New X coordinate position.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// New Y coordinate position.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// New Z coordinate position.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// New orientation of the whiteboard.
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// New color of the whiteboard marker.
    /// </summary>
    public string MarkerColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWhiteboardDto"/> class.
    /// </summary>
    public UpdateWhiteboardDto(
        string whiteboardId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor)
    {
        WhiteboardId = whiteboardId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
        MarkerColor = markerColor;
    }
}
