namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a building in the theme park UCR.
/// </summary>
public class Building
{
    /// <summary>
    /// Gets the internal identifier for the building.
    /// </summary>
    public int InternalId { get; private set; }

    /// <summary>
    /// Gets the name of the building.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the color of the building.
    /// </summary>
    public string Color { get; private set; }

    /// <summary>
    /// Gets the height of the building in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Gets the length of the building in meters.
    /// </summary>
    public float Length { get; private set; }

    /// <summary>
    /// Gets the width of the building in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// Gets the X coordinate of the building position.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Gets the Y coordinate of the building position.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Gets the Z coordinate of the building position.
    /// </summary>
    public float Z { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private Building()
    {
        Name = string.Empty;
        Color = string.Empty;
    }

    /// <summary>
    /// Constructor for the Building class.
    /// </summary>
    /// <param name="internalId">The internal identifier.</param>
    /// <param name="name">The name of the building.</param>
    /// <param name="color">The color of the building.</param>
    /// <param name="height">The height of the building in meters.</param>
    /// <param name="length">The length of the building in meters.</param>
    /// <param name="width">The width of the building in meters.</param>
    /// <param name="x">The X coordinate of the building position.</param>
    /// <param name="y">The Y coordinate of the building position.</param>
    /// <param name="z">The Z coordinate of the building position.</param>
    public Building(int internalId, string name, string color, float height, float length, float width, float x, float y, float z)
    {
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
