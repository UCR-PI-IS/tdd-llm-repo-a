namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a building in the theme park UCR.
/// </summary>
public class Building
{
    /// <summary>
    /// Unique internal identifier for the building.
    /// </summary>
    public int InternalId { get; private set; }

    /// <summary>
    /// Name of the building.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Color of the building.
    /// </summary>
    public string Color { get; private set; }

    /// <summary>
    /// Height of the building in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Length of the building in meters.
    /// </summary>
    public float Length { get; private set; }

    /// <summary>
    /// Width of the building in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// X coordinate of the building.
    /// </summary>
    public float X { get; private set; }

    /// <summary>
    /// Y coordinate of the building.
    /// </summary>
    public float Y { get; private set; }

    /// <summary>
    /// Z coordinate of the building.
    /// </summary>
    public float Z { get; private set; }

    /// <summary>
    /// Identifier of the area this building belongs to.
    /// </summary>
    public int AreaId { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private Building()
    {
        Name = string.Empty;
        Color = string.Empty;
    }

    /// <summary>
    /// Constructor for the Building class with internal identifier.
    /// </summary>
    /// <param name="internalId">Unique internal identifier</param>
    /// <param name="name">Name of the building</param>
    /// <param name="color">Color of the building</param>
    /// <param name="height">Height in meters</param>
    /// <param name="length">Length in meters</param>
    /// <param name="width">Width in meters</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
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

    /// <summary>
    /// Constructor for the Building class with validation.
    /// </summary>
    /// <param name="name">Name of the building</param>
    /// <param name="color">Color of the building</param>
    /// <param name="height">Height in meters (must be positive)</param>
    /// <param name="length">Length in meters (must be positive)</param>
    /// <param name="width">Width in meters (must be positive)</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="areaId">Area identifier (must be positive)</param>
    /// <exception cref="ArgumentException">Thrown when any validation rule is violated.</exception>
    public Building(string name, string color, float height, float length, float width, float x, float y, float z, int areaId)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrEmpty(color))
            throw new ArgumentException("Color cannot be empty", nameof(color));

        ThrowIfNotPositive(height, nameof(height));
        ThrowIfNotPositive(length, nameof(length));
        ThrowIfNotPositive(width, nameof(width));
        ThrowIfNotPositive(areaId, nameof(areaId));

        Name = name;
        Color = color;
        Height = height;
        Length = length;
        Width = width;
        X = x;
        Y = y;
        Z = z;
        AreaId = areaId;
    }

    private static void ThrowIfNotPositive(float value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }

    private static void ThrowIfNotPositive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }
}
