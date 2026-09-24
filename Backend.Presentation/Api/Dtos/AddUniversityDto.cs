namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for adding a university.
/// </summary>
/// <param name="Name">The name of the university.</param>
/// <param name="Country">The country of the university.</param>
public record class AddUniversityDto(
    string Name,
    string Country);
