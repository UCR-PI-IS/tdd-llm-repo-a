namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new person.
/// </summary>
public class CreatePersonDto
{
    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Gets the last name of the person.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Gets the email address of the person.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the identity number of the person.
    /// </summary>
    public string IdentityNumber { get; }

    /// <summary>
    /// Gets the birth date of the person.
    /// </summary>
    public DateTime BirthDate { get; }

    /// <summary>
    /// Gets the phone number of the person (optional).
    /// </summary>
    public string? Phone { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonDto"/> class.
    /// </summary>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>
    /// <param name="email">The email address of the person.</param>
    /// <param name="identityNumber">The identity number of the person.</param>
    /// <param name="birthDate">The birth date of the person.</param>
    /// <param name="phone">The phone number of the person (optional).</param>
    public CreatePersonDto(string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Phone = phone;
    }
}
