namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "South", "East", "West"
    };

    private static readonly HashSet<string> ValidColors = new()
    {
        "Red", "Blue", "Green", "Black", "White", "Yellow", "Purple", "Orange"
    };

    /// <summary>
    /// Unique identifier for the whiteboard.
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
    /// Color of the whiteboard marker.
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
    /// <param name="learningSpaceId">Identifier of the learning space this whiteboard belongs to.</param>
    /// <param name="width">Width of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="height">Height of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="depth">Depth of the whiteboard in meters. Must be non-negative.</param>
    /// <param name="x">X coordinate position. Must be non-negative.</param>
    /// <param name="y">Y coordinate position. Must be non-negative.</param>
    /// <param name="z">Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">Orientation of the whiteboard. Must be North, South, East, or West.</param>
    /// <param name="markerColor">Color of the whiteboard marker. Must be a valid color name.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, orientation is invalid, or markerColor is invalid.</exception>
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
        ValidateAllProperties(width, height, depth, x, y, z, orientation, markerColor);

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
    /// Updates the whiteboard properties with new values after validating them.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    public void Update(
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor)
    {
        ValidateAllProperties(width, height, depth, x, y, z, orientation, markerColor);

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
    /// Updates the whiteboard and validates that the new position does not exceed
    /// the learning space boundaries.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="learningSpaceWidth">Width of the learning space.</param>
    /// <param name="learningSpaceLength">Length of the learning space.</param>
    /// <exception cref="InvalidOperationException">Thrown when the new position exceeds learning space boundaries.</exception>
    public void Update(
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor,
        float learningSpaceWidth,
        float learningSpaceLength)
    {
        if (x + width > learningSpaceWidth || z + depth > learningSpaceLength)
            throw new InvalidOperationException("Position exceeds learning space boundaries");

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard and validates that the new position does not overlap
    /// with any existing component in the same learning space.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="existingComponents">List of existing components in the same learning space.</param>
    /// <exception cref="InvalidOperationException">Thrown when the new position overlaps with an existing component.</exception>
    public void Update(
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string markerColor,
        List<Whiteboard> existingComponents)
    {
        foreach (var other in existingComponents)
        {
            if (other.ComponentId == ComponentId)
                continue;

            if (Overlaps(x, y, z, width, height, depth, other.X, other.Y, other.Z, other.Width, other.Height, other.Depth))
                throw new InvalidOperationException("Position overlaps with existing component");
        }

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Determines whether this whiteboard fits within the specified learning space,
    /// considering both dimensions and position.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>True if the whiteboard fits within the learning space; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        return FitsAxis(Width, X, learningSpace.Width)
            && FitsAxis(Height, Y, learningSpace.Height)
            && FitsAxis(Depth, Z, learningSpace.Length);
    }

    private static bool Overlaps(
        float x1, float y1, float z1, float w1, float h1, float d1,
        float x2, float y2, float z2, float w2, float h2, float d2)
    {
        return AxisOverlaps(x1, w1, x2, w2)
            && AxisOverlaps(y1, h1, y2, h2)
            && AxisOverlaps(z1, d1, z2, d2);
    }

    private static bool AxisOverlaps(float pos1, float size1, float pos2, float size2)
    {
        return pos1 < pos2 + size2 && pos2 < pos1 + size1;
    }

    private static bool FitsAxis(float size, float position, float spaceSize)
    {
        return size <= spaceSize && position + size <= spaceSize;
    }

    private static void ValidateAllProperties(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width, nameof(width));
        ArgumentOutOfRangeException.ThrowIfNegative(height, nameof(height));
        ArgumentOutOfRangeException.ThrowIfNegative(depth, nameof(depth));
        ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
        ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
        ArgumentOutOfRangeException.ThrowIfNegative(z, nameof(z));
        ValidateOrientation(orientation);
        ValidateMarkerColor(markerColor);
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
        ArgumentException.ThrowIfNullOrEmpty(markerColor, nameof(markerColor));
        if (!ValidColors.Contains(markerColor))
            throw new ArgumentException(
                $"Invalid color name '{markerColor}'. Must be one of: Red, Blue, Green, Black, White, Yellow, Purple, Orange.",
                nameof(markerColor));
    }
}
