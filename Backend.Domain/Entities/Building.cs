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
    /// Identifier of the area the building belongs to.
    /// </summary>
    public int AreaId { get; private set; }

    /// <summary>
    /// Validates that a string is not null or empty.
    /// </summary>
    private static void ValidateNotNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"{paramName} cannot be empty", paramName);
    }

    /// <summary>
    /// Validates that a dimension value is positive.
    /// </summary>
    private static void ValidatePositiveDimension(float value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }

    /// <summary>
    /// Validates that an integer value is positive.
    /// </summary>
    private static void ValidatePositiveInt(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }

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
    /// <exception cref="ArgumentException">Thrown when any validation rule is violated.</exception>
    public Building(int internalId, string name, string color, float height, float length, float width, float x, float y, float z)
    {
        ValidateNotNullOrEmpty(name, nameof(name));
        ValidateNotNullOrEmpty(color, nameof(color));
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(length, nameof(length));
        ValidatePositiveDimension(width, nameof(width));

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
    /// Constructor for the Building class with area reference (no validation).
    /// Used by the presentation layer to defer validation to the application service.
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
    /// <param name="areaId">Identifier of the area</param>
    public Building(int internalId, string name, string color, float height, float length, float width, float x, float y, float z, int areaId)
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
        AreaId = areaId;
    }

    /// <summary>
    /// Constructor for the Building class with area reference and validation.
    /// </summary>
    /// <param name="name">Name of the building (must not be null or empty)</param>
    /// <param name="color">Color of the building (must not be null or empty)</param>
    /// <param name="height">Height in meters (must be positive)</param>
    /// <param name="length">Length in meters (must be positive)</param>
    /// <param name="width">Width in meters (must be positive)</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <param name="areaId">Identifier of the area (must be positive)</param>
    /// <exception cref="ArgumentException">Thrown when any validation rule is violated.</exception>
    public Building(string name, string color, float height, float length, float width, float x, float y, float z, int areaId)
    {
        ValidateNotNullOrEmpty(name, nameof(name));
        ValidateNotNullOrEmpty(color, nameof(color));
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(length, nameof(length));
        ValidatePositiveDimension(width, nameof(width));
        ValidatePositiveInt(areaId, nameof(areaId));

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
    /// Updates the building properties with new values.
    /// </summary>
    /// <param name="name">New name of the building (must not be null or empty)</param>
    /// <param name="color">New color of the building (must not be null or empty)</param>
    /// <param name="height">New height in meters (must be positive)</param>
    /// <param name="length">New length in meters (must be positive)</param>
    /// <param name="width">New width in meters (must be positive)</param>
    /// <param name="x">New X coordinate</param>
    /// <param name="y">New Y coordinate</param>
    /// <param name="z">New Z coordinate</param>
    /// <exception cref="ArgumentException">Thrown when any validation rule is violated.</exception>
    public void Update(string name, string color, float height, float length, float width, float x, float y, float z)
    {
        ValidateNotNullOrEmpty(name, nameof(name));
        ValidateNotNullOrEmpty(color, nameof(color));
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(length, nameof(length));
        ValidatePositiveDimension(width, nameof(width));

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
