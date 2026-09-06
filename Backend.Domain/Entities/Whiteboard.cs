namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    /// <summary>
    /// Unique identifier for the whiteboard component.
    /// </summary>
    public string ComponentId { get; private set; }

    /// <summary>
    /// Identifier of the learning space this whiteboard belongs to.
    /// </summary>
    public string LearningSpaceId { get; private set; }

    /// <summary>
    /// Width of the whiteboard in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// Height of the whiteboard in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Depth of the whiteboard in meters.
    /// </summary>
    public float Depth { get; private set; }

    /// <summary>
    /// X coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Y coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Z coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float Z { get; private set; }

    /// <summary>
    /// Orientation of the whiteboard (North, South, East, or West).
    /// </summary>
    public string Orientation { get; private set; }

    /// <summary>
    /// Color of the whiteboard markers.
    /// </summary>
    public string MarkerColor { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private Whiteboard()
    {
        ComponentId = string.Empty;
        LearningSpaceId = string.Empty;
        Orientation = string.Empty;
        MarkerColor = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Whiteboard"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the whiteboard.</param>
    /// <param name="learningSpaceId">Identifier of the learning space.</param>
    /// <param name="width">Width of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="height">Height of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="depth">Depth of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="x">X coordinate position. Must be non-negative.</param>
    /// <param name="y">Y coordinate position. Must be non-negative.</param>
    /// <param name="z">Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">Orientation of the whiteboard. Must be North, South, East, or West.</param>
    /// <param name="markerColor">Color of the markers. Must not be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when any validation fails.</exception>
    public Whiteboard(
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
        ArgumentOutOfRangeException.ThrowIfNegative(width, nameof(width));
        ArgumentOutOfRangeException.ThrowIfNegative(height, nameof(height));
        ArgumentOutOfRangeException.ThrowIfNegative(depth, nameof(depth));
        ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
        ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
        ArgumentOutOfRangeException.ThrowIfNegative(z, nameof(z));

        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: North, South, East, West.",
                nameof(orientation));

        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Marker color is required.", nameof(markerColor));

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

    /// <summary>
    /// Determines whether this whiteboard fits within the specified learning space.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>true if the whiteboard fits; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        if (learningSpace == null)
            return false;

        return Math.Max(Width, X + Width) <= learningSpace.Width
            && Math.Max(Height, Y + Height) <= learningSpace.Height
            && Math.Max(Depth, Z + Depth) <= learningSpace.Length;
    }
}
