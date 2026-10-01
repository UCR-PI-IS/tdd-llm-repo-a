namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Data transfer object for updating an existing building.
/// </summary>
public class UpdateBuildingDto
{
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
    /// X coordinate of the building.
    /// </summary>
    public float X { get; set; }

    /// <summary>
    /// Y coordinate of the building.
    /// </summary>
    public float Y { get; set; }

    /// <summary>
    /// Z coordinate of the building.
    /// </summary>
    public float Z { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuildingDto"/> class.
    /// </summary>
    public UpdateBuildingDto(string name, string color, float height, float length, float width, float x, float y, float z)
    {
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
