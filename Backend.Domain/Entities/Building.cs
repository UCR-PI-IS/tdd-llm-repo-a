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
    /// Area identifier where the building is located.
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
    /// Constructor for the Building class.
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
        AreaId = 0;
    }

    /// <summary>
    /// Constructor for the Building class with area validation.
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
    /// <param name="areaId">Area identifier</param>
    public Building(int internalId, string name, string color, float height, float length, float width, float x, float y, float z, int areaId)
    {
        ValidateRequiredString(name, nameof(name));
        ValidateRequiredString(color, nameof(color));
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(length, nameof(length));
        ValidatePositiveDimension(width, nameof(width));
        ValidatePositiveId(areaId, nameof(areaId));

        InternalId = internalId;
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

    /// <summary>
    /// Validates that a string parameter is not null or whitespace.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or whitespace.</exception>
    private static void ValidateRequiredString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }

    /// <summary>
    /// Validates that a dimension parameter is positive.
    /// </summary>
    /// <param name="value">The dimension value to validate.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not positive.</exception>
    private static void ValidatePositiveDimension(float value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be greater than zero.", paramName);
    }

    /// <summary>
    /// Validates that an ID parameter is positive.
    /// </summary>
    /// <param name="value">The ID value to validate.</param>
    /// <param name="paramName">The name of the parameter.</param>
    /// <exception cref="ArgumentException">Thrown when the value is not positive.</exception>
    private static void ValidatePositiveId(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be greater than zero.", paramName);
    }
}
