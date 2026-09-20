namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response model for a successful whiteboard update.
/// </summary>
public class UpdateWhiteboardResponse
{
    /// <summary>
    /// Gets the unique identifier of the updated whiteboard.
    /// </summary>
    public string ComponentId { get; }

    /// <summary>
    /// Gets the identifier of the learning space the whiteboard belongs to.
    /// </summary>
    public string LearningSpaceId { get; }

    /// <summary>
    /// Gets the width of the whiteboard.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Gets the height of the whiteboard.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Gets the depth of the whiteboard.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// Gets the X coordinate of the whiteboard.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Gets the Y coordinate of the whiteboard.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Gets the Z coordinate of the whiteboard.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Gets the orientation of the whiteboard.
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Gets the marker color of the whiteboard.
    /// </summary>
    public string MarkerColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWhiteboardResponse"/> class.
    /// </summary>
    /// <param name="componentId">The component identifier.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="depth">The depth.</param>
    /// <param name="x">The X coordinate.</param>
    /// <param name="y">The Y coordinate.</param>
    /// <param name="z">The Z coordinate.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="markerColor">The marker color.</param>
    public UpdateWhiteboardResponse(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor)
    {
        ComponentId = componentId;
        LearningSpaceId = learningSpaceId;
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
