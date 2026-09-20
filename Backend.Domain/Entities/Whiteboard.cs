namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// Extends <see cref="LearningComponent"/> with marker color information.
/// </summary>
public class Whiteboard : LearningComponent
{
    private static readonly HashSet<string> ValidColors = new()
    {
        "Red", "Blue", "Green", "Black", "White",
        "Yellow", "Orange", "Purple", "Brown", "Pink", "Gray"
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
        : base(componentId, learningSpaceId, width, height, depth, x, y, z, orientation)
    {
        ValidateMarkerColor(markerColor);
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
    /// Updates the whiteboard properties with the specified values.
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
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string markerColor)
    {
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
    /// Updates the whiteboard properties and validates that the new position
    /// does not exceed the learning space boundaries.
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
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string markerColor,
        float learningSpaceWidth, float learningSpaceLength)
    {
        if (x + width > learningSpaceWidth || z + depth > learningSpaceLength)
            throw new InvalidOperationException("Position exceeds learning space boundaries");

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard properties and validates that the new position
    /// does not overlap with existing components.
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
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string markerColor,
        List<LearningComponent> existingComponents)
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

    public static bool Overlaps(
        float x1, float z1, float w1, float d1,
        float x2, float z2, float w2, float d2)
    {
        return x1 < x2 + w2 && x1 + w1 > x2
            && z1 < z2 + d2 && z1 + d1 > z2;
    }

    private static void ValidateMarkerColor(string markerColor)
    {
        ArgumentException.ThrowIfNullOrEmpty(markerColor, nameof(markerColor));

        if (!ValidColors.Contains(markerColor))
            throw new ArgumentException(
                $"Invalid color name '{markerColor}'. Must be one of: {string.Join(", ", ValidColors)}.",
                nameof(markerColor));
    }
}
