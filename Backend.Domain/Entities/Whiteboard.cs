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
        "Red", "Green", "Blue", "Black", "White", "Yellow"
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
        ValidatePhysicalProperties(width, height, depth, x, y, z, orientation, markerColor);

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
        return Width + X <= learningSpace.Width
            && Height + Y <= learningSpace.Height
            && Depth + Z <= learningSpace.Length;
    }

    private static void ValidatePhysicalProperties(
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string markerColor)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(width, nameof(width));
        ArgumentOutOfRangeException.ThrowIfNegative(height, nameof(height));
        ArgumentOutOfRangeException.ThrowIfNegative(depth, nameof(depth));
        ArgumentOutOfRangeException.ThrowIfNegative(x, nameof(x));
        ArgumentOutOfRangeException.ThrowIfNegative(y, nameof(y));
        ArgumentOutOfRangeException.ThrowIfNegative(z, nameof(z));

        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: South, East, West.",
                nameof(orientation));

        ArgumentException.ThrowIfNullOrEmpty(markerColor, nameof(markerColor));
        if (!ValidColors.Contains(markerColor))
            throw new ArgumentException(
                $"Invalid color name '{markerColor}'. Must be one of: {string.Join(", ", ValidColors)}.",
                nameof(markerColor));
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
        ValidatePhysicalProperties(width, height, depth, x, y, z, orientation, markerColor);

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
    /// Updates the whiteboard properties with new values and validates against learning space boundaries.
    /// </summary>
    /// <param name="width">New width of the whiteboard.</param>
    /// <param name="height">New height of the whiteboard.</param>
    /// <param name="depth">New depth of the whiteboard.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="learningSpaceWidth">Width of the learning space for boundary validation.</param>
    /// <param name="learningSpaceLength">Length of the learning space for boundary validation.</param>
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
        PositionValidator.ValidateBoundary(x, z, width, depth, learningSpaceWidth, learningSpaceLength);
        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Updates the whiteboard properties with new values and validates against existing components.
    /// </summary>
    /// <param name="width">New width of the whiteboard.</param>
    /// <param name="height">New height of the whiteboard.</param>
    /// <param name="depth">New depth of the whiteboard.</param>
    /// <param name="x">New X coordinate.</param>
    /// <param name="y">New Y coordinate.</param>
    /// <param name="z">New Z coordinate.</param>
    /// <param name="orientation">New orientation.</param>
    /// <param name="markerColor">New marker color.</param>
    /// <param name="existingComponents">List of existing components to check for overlaps.</param>
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
        IEnumerable<LearningComponent> existingComponents)
    {
        PositionValidator.ValidateNoOverlap(this.ComponentId, x, z, width, depth, existingComponents);
        Update(width, height, depth, x, y, z, orientation, markerColor);
    }

    /// <summary>
    /// Implicitly converts a Whiteboard to a LearningComponent.
    /// </summary>
    /// <param name="whiteboard">The whiteboard to convert.</param>
    public static implicit operator LearningComponent(Whiteboard whiteboard)
    {
        return new LearningComponent(
            whiteboard.ComponentId,
            whiteboard.LearningSpaceId,
            whiteboard.Width,
            whiteboard.Height,
            whiteboard.Depth,
            whiteboard.X,
            whiteboard.Y,
            whiteboard.Z,
            whiteboard.Orientation);
    }
}
