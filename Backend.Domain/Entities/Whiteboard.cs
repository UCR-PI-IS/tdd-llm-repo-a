namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a whiteboard in a learning space.
/// </summary>
public class Whiteboard
{
    /// <summary>
    /// Unique identifier for the whiteboard.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Width of the whiteboard in meters.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// Height of the whiteboard in meters.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Constructor for the Whiteboard class.
    /// </summary>
    /// <param name="id">Unique identifier for the whiteboard</param>
    /// <param name="width">Width of the whiteboard in meters</param>
    /// <param name="height">Height of the whiteboard in meters</param>
    /// <exception cref="ArgumentException">Thrown when id is null/empty or dimensions are invalid</exception>
    public Whiteboard(string id, double width, double height)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        if (width <= 0)
            throw new ArgumentException("Width must be positive", nameof(width));

        if (height <= 0)
            throw new ArgumentException("Height must be positive", nameof(height));

        Id = id;
        Width = width;
        Height = height;
    }
}
