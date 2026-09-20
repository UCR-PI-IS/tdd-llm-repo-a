namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard : LearningComponent
{
    private static readonly HashSet<string> ValidMarkerColors = new()
    {
        "Red", "Blue", "Green", "Black", "White",
        "Yellow", "Orange", "Purple", "Brown", "Pink"
    };

    private static readonly HashSet<string> WhiteboardValidOrientations = new()
    {
        "South", "East", "West"
    };

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
        : base(componentId, learningSpaceId, width, height, depth, x, y, z, orientation)
    {
        ValidateWhiteboardOrientation(orientation);
        ValidateMarkerColor(markerColor);
        MarkerColor = markerColor;
    }

    /// <summary>
    /// Updates all whiteboard properties with the specified values after validation.
    /// </summary>
    /// <param name="width">New width in meters.</param>
    /// <param name="height">New height in meters.</param>
    /// <param name="depth">New depth in meters.</param>
    /// <param name="x">New X coordinate position.</param>
    /// <param name="y">New Y coordinate position.</param>
    /// <param name="z">New Z coordinate position.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, orientation is invalid, or markerColor is invalid.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor)
    {
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
        ValidateWhiteboardOrientation(orientation);
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
    /// Updates the whiteboard after validating that the new position does not exceed
    /// the learning space boundaries.
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
    /// Updates the whiteboard after validating that the new position does not overlap
    /// with any existing component in the same learning space.
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
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, List<LearningComponent> existingComponents)
    {
        if (existingComponents.Any(c => c.ComponentId != ComponentId && Overlaps(c, x, z, width, depth)))
            throw new InvalidOperationException("Position overlaps with existing component");

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

    private static bool FitsAxis(float size, float position, float spaceSize)
    {
        return size <= spaceSize && position + size <= spaceSize;
    }

    private static bool Overlaps(LearningComponent other, float newX, float newZ, float newWidth, float newDepth)
    {
        return newX < other.X + other.Width
            && newX + newWidth > other.X
            && newZ < other.Z + other.Depth
            && newZ + newDepth > other.Z;
    }

    private static void ValidateWhiteboardOrientation(string orientation)
    {
        if (!WhiteboardValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: South, East, West.",
                nameof(orientation));
    }

    private static void ValidateMarkerColor(string markerColor)
    {
        ArgumentException.ThrowIfNullOrEmpty(markerColor, nameof(markerColor));
        if (!ValidMarkerColors.Contains(markerColor))
            throw new ArgumentException(
                $"Invalid color name '{markerColor}'. Must be one of: {string.Join(", ", ValidMarkerColors)}.",
                nameof(markerColor));
    }
}
