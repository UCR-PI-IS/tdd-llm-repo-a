namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for adding a new university.
/// </summary>
public class AddUniversityDto
{
    /// <summary>
    /// Gets or sets the name of the university.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country where the university is located.
    /// </summary>
    public string Country { get; set; } = string.Empty;
}
