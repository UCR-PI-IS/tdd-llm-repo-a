namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a building in the theme park UCR.
/// </summary>
public class Building
{
    /// <summary>
    /// Unique internal identifier for the building.
    /// </summary>
    public int InternalId { get; set; }

    /// <summary>
    /// Name of the building.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Color of the building.
    /// </summary>
    public string Color { get; set; }

    /// <summary>
    /// Height of the building in meters.
    /// </summary>
    public float Height { get; set; }

    /// <summary>
    /// Length of the building in meters.
    /// </summary>
    public float Length { get; set; }

    /// <summary>
    /// Width of the building in meters.
    /// </summary>
    public float Width { get; set; }

    /// <summary>
    /// X coordinate of the building position.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate of the building position.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate of the building position.
    /// </summary>
    public float Z { get; set; }

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
    /// <param name="internalId">Unique internal identifier.</param>
    /// <param name="name">Name of the building.</param>
    /// <param name="color">Color of the building.</param>
    /// <param name="height">Height of the building in meters.</param>
    /// <param name="length">Length of the building in meters.</param>
    /// <param name="width">Width of the building in meters.</param>
    /// <param name="x">X coordinate of the building position.</param>
    /// <param name="y">Y coordinate of the building position.</param>
    /// <param name="z">Z coordinate of the building position.</param>
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
