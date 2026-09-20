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

    private static readonly HashSet<string> ValidColors = new(StringComparer.OrdinalIgnoreCase)
    {
        "Blue", "Red", "Green", "Black", "White", "Yellow", "Orange", "Purple", "Brown", "Pink"
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
    /// Orientation of the whiteboard (South, East, or West).
    /// </summary>
    public string Orientation { get; private set; }

    /// <summary>
    /// Color of the whiteboard marker.
    /// </summary>
    public string MarkerColor { get; private set; }

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
    /// <param name="orientation">Orientation of the whiteboard. Must be South, East, or West.</param>
    /// <param name="markerColor">Color of the whiteboard marker. Must not be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, orientation is invalid, or markerColor is null or empty.</exception>
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

    private static bool FitsAxis(float size, float position, float spaceSize)
    {
        return size <= spaceSize && position + size <= spaceSize;
    }

    /// <summary>
    /// Updates all whiteboard properties with the specified values.
    /// </summary>
    /// <param name="width">New width in meters. Must be non-negative.</param>
    /// <param name="height">New height in meters. Must be non-negative.</param>
    /// <param name="depth">New depth in meters. Must be non-negative.</param>
    /// <param name="x">New X coordinate position. Must be non-negative.</param>
    /// <param name="y">New Y coordinate position. Must be non-negative.</param>
    /// <param name="z">New Z coordinate position. Must be non-negative.</param>
    /// <param name="orientation">New orientation. Must be South, East, or West.</param>
    /// <param name="markerColor">New marker color. Must be a valid color name.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, orientation is invalid, or markerColor is invalid.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor)
    {
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
        ValidateOrientation(orientation);
        ValidateMarkerColor(markerColor);

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
    /// Updates the whiteboard with boundary validation against the learning space dimensions.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate position.</param>
    /// <param name="y">New Y coordinate position.</param>
    /// <param name="z">New Z coordinate position.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="learningSpaceWidth">Width of the learning space.</param>
    /// <param name="learningSpaceLength">Length of the learning space.</param>
    /// <exception cref="InvalidOperationException">Thrown when the new position exceeds learning space boundaries.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, float learningSpaceWidth, float learningSpaceLength)
    {
        if (x > learningSpaceWidth || z > learningSpaceLength)
            throw new InvalidOperationException("Position exceeds learning space boundaries");

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard with overlap validation against existing components.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate position.</param>
    /// <param name="y">New Y coordinate position.</param>
    /// <param name="z">New Z coordinate position.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="existingComponents">List of existing components in the same learning space.</param>
    /// <exception cref="InvalidOperationException">Thrown when the new position overlaps with an existing component.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, List<Whiteboard> existingComponents)
    {
        foreach (var component in existingComponents)
        {
            if (component.ComponentId == ComponentId)
                continue;

            if (Overlaps(x, z, width, depth, component.X, component.Z, component.Width, component.Depth))
                throw new InvalidOperationException("Position overlaps with existing component");
        }

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    private static bool Overlaps(float x1, float z1, float w1, float d1, float x2, float z2, float w2, float d2)
    {
        return x1 < x2 + w2 && x1 + w1 > x2 && z1 < z2 + d2 && z1 + d1 > z2;
    }

    private static void ThrowIfNegative(float value, string paramName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value, paramName);
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: South, East, West.",
                nameof(orientation));
    }

    private static void ValidateMarkerColor(string markerColor)
    {
        ArgumentException.ThrowIfNullOrEmpty(markerColor, nameof(markerColor));
        if (!ValidColors.Contains(markerColor))
            throw new ArgumentException(
                $"Invalid color name '{markerColor}'. Must be one of the valid colors.",
                nameof(markerColor));
    }
}
