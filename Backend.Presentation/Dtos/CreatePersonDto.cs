namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a person.
/// </summary>
/// <param name="FirstName">First name of the person.</param>
/// <param name="LastName">Last name of the person.</param>
/// <param name="Email">Email address of the person.</param>
/// <param name="IdentityNumber">Identity number of the person.</param>
/// <param name="BirthDate">Birth date of the person.</param>
public record class CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string IdentityNumber,
    DateTime BirthDate);
