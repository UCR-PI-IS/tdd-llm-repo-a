namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard : LearningComponent
{
    private static readonly HashSet<string> ValidWhiteboardOrientations = new()
    {
        "South", "East", "West"
    };

    private static readonly HashSet<string> ValidColors = new()
    {
        "Blue", "Red", "Green", "Black", "White"
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
        : base(componentId, learningSpaceId, width, height, depth, x, y, z, orientation)
    {
        ValidateWhiteboardSpecificProperties(orientation, markerColor);
        MarkerColor = markerColor;
    }

    /// <summary>
    /// Updates the whiteboard properties with new values.
    /// </summary>
    /// <param name="width">New width.</param>
    /// <param name="height">New height.</param>
    /// <param name="depth">New depth.</param>
    /// <param name="x">New X position.</param>
    /// <param name="y">New Y position.</param>
    /// <param name="z">New Z position.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor)
    {
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
        ValidateWhiteboardSpecificProperties(orientation, markerColor);

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
    /// fits within the learning space boundaries.
    /// </summary>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, float learningSpaceWidth, float learningSpaceLength)
    {
        bool exceedsWidth = x > learningSpaceWidth;
        bool exceedsLength = z > learningSpaceLength;
        if (exceedsWidth | exceedsLength)
            throw new InvalidOperationException("Position exceeds learning space boundaries");

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard properties and validates that the new position
    /// does not overlap with existing components.
    /// </summary>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, List<LearningComponent> existingComponents)
    {
        SpatialValidator.ThrowIfOverlaps(x, y, z, width, height, depth, ComponentId, existingComponents);
        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    private static void ValidateWhiteboardSpecificProperties(string orientation, string markerColor)
    {
        if (!ValidWhiteboardOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: South, East, West.",
                nameof(orientation));

        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Invalid marker color", nameof(markerColor));

        if (!ValidColors.Contains(markerColor))
            throw new ArgumentException("Invalid color name", nameof(markerColor));
    }

    /// <summary>
    /// Determines whether this whiteboard fits within the specified learning space,
    /// considering both dimensions and position.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>True if the whiteboard fits within the learning space; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        bool fitsX = SpatialValidator.FitsAxis(Width, X, learningSpace.Width);
        bool fitsY = SpatialValidator.FitsAxis(Height, Y, learningSpace.Height);
        bool fitsZ = SpatialValidator.FitsAxis(Depth, Z, learningSpace.Length);
        return fitsX & fitsY & fitsZ;
    }
}
