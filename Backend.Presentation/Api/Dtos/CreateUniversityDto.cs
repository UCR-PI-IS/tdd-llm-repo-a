namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a university.
/// </summary>
/// <param name="Name">The name of the university.</param>
/// <param name="Country">The country where the university is located.</param>
public record class CreateUniversityDto(
    string Name,
    string Country);
