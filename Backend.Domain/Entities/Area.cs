namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents an area in the theme park.
/// </summary>
public class Area
{
    /// <summary>
    /// Unique identifier for the area.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the area.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
