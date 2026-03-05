namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

/// <summary>
/// Represents a request to add a new building.
/// </summary>
public class AddBuildingRequest
{
    /// <summary>
    /// Gets or sets the name of the building.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the color of the building.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets the height of the building.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the length of the building.
    /// </summary>
    public double Length { get; set; }

    /// <summary>
    /// Gets or sets the width of the building.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate of the building.
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the building.
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the Z coordinate of the building.
    /// </summary>
    public double Z { get; set; }
}
