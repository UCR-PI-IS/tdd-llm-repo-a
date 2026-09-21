namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a person.
/// </summary>
/// <param name="FirstName">The first name of the person.</param>
/// <param name="LastName">The last name of the person.</param>
/// <param name="Email">The email address of the person.</param>
/// <param name="IdentityNumber">The identity number of the person.</param>
/// <param name="BirthDate">The birth date of the person.</param>
public record class CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string IdentityNumber,
    DateTime BirthDate);
