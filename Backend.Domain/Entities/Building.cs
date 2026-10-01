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
        BuildingValidation.ValidateRequiredString(name, nameof(name));
        BuildingValidation.ValidateRequiredString(color, nameof(color));
        BuildingValidation.ValidatePositive(height, nameof(height));
        BuildingValidation.ValidatePositive(length, nameof(length));
        BuildingValidation.ValidatePositive(width, nameof(width));
        BuildingValidation.ValidatePositive(areaId, nameof(areaId));

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
    /// Updates the building properties with validation.
    /// </summary>
    /// <param name="name">Name of the building (must not be null or empty)</param>
    /// <param name="color">Color of the building (must not be null or empty)</param>
    /// <param name="height">Height in meters (must be positive)</param>
    /// <param name="length">Length in meters (must be positive)</param>
    /// <param name="width">Width in meters (must be positive)</param>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    /// <param name="z">Z coordinate</param>
    /// <exception cref="ArgumentException">Thrown when any validation rule is violated.</exception>
    public void Update(string name, string color, float height, float length, float width, float x, float y, float z)
    {
        BuildingValidation.ValidateRequiredString(name, nameof(name));
        BuildingValidation.ValidateRequiredString(color, nameof(color));
        BuildingValidation.ValidatePositive(height, nameof(height));
        BuildingValidation.ValidatePositive(length, nameof(length));
        BuildingValidation.ValidatePositive(width, nameof(width));

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
