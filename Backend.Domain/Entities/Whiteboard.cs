namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard : LearningComponent
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "South", "East", "West"
    };

    private static readonly HashSet<string> ValidColors = new()
    {
        "Red", "Green", "Blue", "Black", "White", "Yellow"
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
        ValidateOrientation(orientation);
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
            throw new ArgumentException($"Invalid color name '{markerColor}'. Must be one of: {string.Join(", ", ValidColors)}.", nameof(markerColor));
    }

    /// <summary>
    /// Updates the whiteboard properties with new values.
    /// </summary>
    /// <param name="width">New width of the whiteboard.</param>
    /// <param name="height">New height of the whiteboard.</param>
    /// <param name="depth">New depth of the whiteboard.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor)
    {
        ValidateDimensions(width, height, depth, x, y, z);
        ValidateOrientation(orientation);
        ValidateMarkerColor(markerColor);

        // Use reflection to set private fields since properties are read-only in base class
        typeof(LearningComponent).GetProperty(nameof(Width))!.SetValue(this, width);
        typeof(LearningComponent).GetProperty(nameof(Height))!.SetValue(this, height);
        typeof(LearningComponent).GetProperty(nameof(Depth))!.SetValue(this, depth);
        typeof(LearningComponent).GetProperty(nameof(X))!.SetValue(this, x);
        typeof(LearningComponent).GetProperty(nameof(Y))!.SetValue(this, y);
        typeof(LearningComponent).GetProperty(nameof(Z))!.SetValue(this, z);
        typeof(LearningComponent).GetProperty(nameof(Orientation))!.SetValue(this, orientation);
        MarkerColor = markerColor;
    }

    private static void ValidateDimensions(float width, float height, float depth, float x, float y, float z)
    {
        ThrowIfNegative(width, nameof(width));
        ThrowIfNegative(height, nameof(height));
        ThrowIfNegative(depth, nameof(depth));
        ThrowIfNegative(x, nameof(x));
        ThrowIfNegative(y, nameof(y));
        ThrowIfNegative(z, nameof(z));
    }

    /// <summary>
    /// Updates the whiteboard properties with boundary checking.
    /// </summary>
    /// <param name="width">New width of the whiteboard.</param>
    /// <param name="height">New height of the whiteboard.</param>
    /// <param name="depth">New depth of the whiteboard.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="learningSpaceWidth">Width of the learning space.</param>
    /// <param name="learningSpaceLength">Length of the learning space.</param>
    /// <exception cref="InvalidOperationException">Thrown when position exceeds learning space boundaries.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, float learningSpaceWidth, float learningSpaceLength)
    {
        // Check if position exceeds boundaries
        if (x + width > learningSpaceWidth || z + depth > learningSpaceLength)
        {
            throw new InvalidOperationException("Position exceeds learning space boundaries");
        }

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard properties with overlap checking.
    /// </summary>
    /// <param name="width">New width of the whiteboard.</param>
    /// <param name="height">New height of the whiteboard.</param>
    /// <param name="depth">New depth of the whiteboard.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="existingComponents">List of existing components to check for overlap.</param>
    /// <exception cref="InvalidOperationException">Thrown when position overlaps with existing component.</exception>
    public void Update(float width, float height, float depth, float x, float y, float z, string orientation, string markerColor, IEnumerable<LearningComponent> existingComponents)
    {
        // Check for overlap with existing components
        foreach (var component in existingComponents)
        {
            if (component.ComponentId != this.ComponentId && IsOverlapping(x, z, width, depth, component))
            {
                throw new InvalidOperationException("Position overlaps with existing component");
            }
        }

        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    private static bool IsOverlapping(float x, float z, float width, float depth, LearningComponent component)
    {
        // Check if two rectangles overlap in 2D space (X-Z plane)
        bool xOverlap = x < component.X + component.Width && x + width > component.X;
        bool zOverlap = z < component.Z + component.Depth && z + depth > component.Z;
        return xOverlap && zOverlap;
    }
}
