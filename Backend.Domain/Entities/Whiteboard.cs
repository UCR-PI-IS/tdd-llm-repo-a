namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Provides validation helper methods for whiteboard entities.
/// </summary>
internal static class WhiteboardValidation
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    public static void ThrowIfNegative(float value, string paramName)
    {
        if (value < 0f)
            throw new ArgumentException($"{paramName} cannot be negative.", paramName);
    }

    public static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: North, South, East, West.",
                nameof(orientation));
    }

    public static void ValidateMarkerColor(string markerColor)
    {
        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Marker color is required.", nameof(markerColor));
    }
}

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// </summary>
public class Whiteboard
{

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
    /// Color of the markers for the whiteboard.
    /// </summary>
    public string MarkerColor { get; }

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
        ValidateDimensions(width, height, depth, x, y, z);
        WhiteboardValidation.ValidateOrientation(orientation);
        WhiteboardValidation.ValidateMarkerColor(markerColor);

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

    private static void ValidateDimensions(float width, float height, float depth, float x, float y, float z)
    {
        WhiteboardValidation.ThrowIfNegative(width, nameof(width));
        WhiteboardValidation.ThrowIfNegative(height, nameof(height));
        WhiteboardValidation.ThrowIfNegative(depth, nameof(depth));
        WhiteboardValidation.ThrowIfNegative(x, nameof(x));
        WhiteboardValidation.ThrowIfNegative(y, nameof(y));
        WhiteboardValidation.ThrowIfNegative(z, nameof(z));
    }

    /// <summary>
    /// Determines whether the whiteboard fits within the specified learning space,
    /// considering both dimensions and position.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>True if the whiteboard fits; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        if (Width > learningSpace.Width)
            return false;
        if (Height > learningSpace.Height)
            return false;
        if (Depth > learningSpace.Length)
            return false;
        if (X + Width > learningSpace.Width)
            return false;
        if (Y + Height > learningSpace.Height)
            return false;
        if (Z + Depth > learningSpace.Length)
            return false;
        return true;
    }
}
