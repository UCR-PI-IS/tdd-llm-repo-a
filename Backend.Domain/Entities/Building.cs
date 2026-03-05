namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a building in the Theme Park system.
/// </summary>
public class Building
{
    /// <summary>
    /// Internal identifier for the building.
    /// </summary>
    public int InternalId { get; }

    /// <summary>
    /// Name of the building.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Color of the building.
    /// </summary>
    public string Color { get; }

    /// <summary>
    /// Height of the building.
    /// </summary>
    public double Height { get; }

    /// <summary>
    /// Length of the building.
    /// </summary>
    public double Length { get; }

    /// <summary>
    /// Width of the building.
    /// </summary>
    public double Width { get; }

    /// <summary>
    /// X coordinate of the building.
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Y coordinate of the building.
    /// </summary>
    public double Y { get; }

    /// <summary>
    /// Z coordinate of the building.
    /// </summary>
    public double Z { get; }

    /// <summary>
    /// Initializes a new instance of the Building class with validation.
    /// </summary>
    public Building(int internalId, string name, string color, double height, double length, double width, double x, double y, double z)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (color == null)
            throw new ArgumentNullException(nameof(color));

        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");

        if (length <= 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be positive");

        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");

        if (x < 0)
            throw new ArgumentOutOfRangeException(nameof(x), "X coordinate cannot be negative");

        if (y < 0)
            throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate cannot be negative");

        if (z < 0)
            throw new ArgumentOutOfRangeException(nameof(z), "Z coordinate cannot be negative");

        InternalId = internalId;
        Name = name;
        Color = color;
        Height = height;
        Length = length;
        Width = width;
        X = x;
        Y = y;
        Z = z;
    }
}
