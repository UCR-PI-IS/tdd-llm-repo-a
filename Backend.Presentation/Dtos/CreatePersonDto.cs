namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a person.
/// </summary>
public record class CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string IdentityNumber,
    DateTime BirthDate);
