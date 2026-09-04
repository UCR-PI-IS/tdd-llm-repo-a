namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard learning component within a learning space.
/// Extends LearningComponent with a marker color property.
/// </summary>
public class Whiteboard : LearningComponent
{
    /// <summary>
    /// Color of the markers for the whiteboard.
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
    /// <param name="markerColor">Color of the markers for the whiteboard.</param>
    /// <exception cref="ArgumentException">Thrown when any dimension or coordinate is negative, orientation is invalid, or markerColor is null/empty.</exception>
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
    /// Checks if the whiteboard fits within the given learning space dimensions and position.
    /// </summary>
    /// <param name="learningSpace">The learning space to check against.</param>
    /// <returns>True if the whiteboard fits within the learning space; otherwise, false.</returns>
    public bool FitsInSpace(LearningSpace learningSpace)
    {
        // Check if whiteboard dimensions exceed space dimensions
        if (Width > learningSpace.Width)
            return false;

        if (Height > learningSpace.Height)
            return false;

        if (Depth > learningSpace.Length)
            return false;

        // Check if whiteboard position + dimensions exceed space boundaries
        if (X + Width > learningSpace.Width)
            return false;

        if (Y + Height > learningSpace.Height)
            return false;

        if (Z + Depth > learningSpace.Length)
            return false;

        return true;
    }

    private static void ValidateMarkerColor(string markerColor)
    {
        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Marker color is required.", nameof(markerColor));
    }
}
