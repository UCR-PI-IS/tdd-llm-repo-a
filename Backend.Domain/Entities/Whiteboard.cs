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
    public string ComponentId { get; }

    /// <summary>
    /// Identifier of the learning space this whiteboard belongs to.
    /// </summary>
    public string LearningSpaceId { get; }

    /// <summary>
    /// Width of the whiteboard in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the whiteboard in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Depth of the whiteboard in meters.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// X coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate of the whiteboard position within the learning space.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the whiteboard (North, South, East, or West).
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Color of the marker for the whiteboard.
    /// </summary>
    public string MarkerColor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Whiteboard"/> class.
    /// </summary>
    /// <param name="componentId">Unique identifier for the whiteboard.</param>
    /// <param name="learningSpaceId">Identifier of the learning space this whiteboard belongs to.</param>
    /// <param name="width">Width of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="height">Height of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="depth">Depth of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="x">X coordinate position. Must be non-negative.</param>
    /// <param name="y">Y coordinate position. Must be non-negative.</param>
    /// <param name="z">Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">Orientation of the whiteboard. Must be North, South, East, or West.</param>
    /// <param name="markerColor">Color of the marker. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
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
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
        ValidateOrientation(orientation);
        ValidateMarkerColor(markerColor);

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

    private static void ThrowIfNegative(float value, string paramName)
    {
        if (value < 0f)
            throw new ArgumentException($"{paramName} cannot be negative.", paramName);
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: North, South, East, West.",
                nameof(orientation));
    }

    private static void ValidateMarkerColor(string markerColor)
    {
        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Marker color is required", nameof(markerColor));
    }

    /// <summary>
    /// Determines whether this whiteboard fits within the specified learning space.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>true if the whiteboard fits within the learning space; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        var fits = Width <= learningSpace.Width;
        fits = fits & (Height <= learningSpace.Height);
        fits = fits & (Depth <= learningSpace.Length);
        fits = fits & (X + Width <= learningSpace.Width);
        fits = fits & (Y + Height <= learningSpace.Height);
        fits = fits & (Z + Depth <= learningSpace.Length);
        return fits;
    }
}
