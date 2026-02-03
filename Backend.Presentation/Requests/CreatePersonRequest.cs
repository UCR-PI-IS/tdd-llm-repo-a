namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

/// <summary>
/// Represents a request to create a new person.
/// </summary>
public class CreatePersonRequest
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the email address of the person.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the identity number of the person.
    /// </summary>
    public string? IdentityNumber { get; set; }

    /// <summary>
    /// Gets or sets the birth date of the person.
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the person.
    /// </summary>
    public string? Phone { get; set; }
}
