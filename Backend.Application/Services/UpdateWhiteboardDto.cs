namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating a whiteboard.
/// </summary>
public class UpdateWhiteboardDto
{
    /// <summary>
    /// Gets the unique identifier of the whiteboard to update.
    /// </summary>
    public string WhiteboardId { get; }

    /// <summary>
    /// Gets the new width of the whiteboard.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Gets the new height of the whiteboard.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Gets the new depth of the whiteboard.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// Gets the new X coordinate.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Gets the new Y coordinate.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Gets the new Z coordinate.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Gets the new orientation.
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Gets the new marker color.
    /// </summary>
    public string MarkerColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWhiteboardDto"/> class.
    /// </summary>
    /// <param name="whiteboardId">The whiteboard identifier.</param>
    /// <param name="width">The new width.</param>
    /// <param name="height">The new height.</param>
    /// <param name="depth">The new depth.</param>
    /// <param name="x">The new X coordinate.</param>
    /// <param name="y">The new Y coordinate.</param>
    /// <param name="z">The new Z coordinate.</param>
    /// <param name="orientation">The new orientation.</param>
    /// <param name="markerColor">The new marker color.</param>
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
