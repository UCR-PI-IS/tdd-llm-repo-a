namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the Theme Park system.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets the unique identifier for the person.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Gets the last name of the person.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Gets the email address of the person.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Gets the identity number of the person.
    /// </summary>
    public string IdentityNumber { get; private set; }

    /// <summary>
    /// Gets the birth date of the person.
    /// </summary>
    public DateTime BirthDate { get; private set; }

    /// <summary>
    /// Gets the optional phone number of the person.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private Person()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        IdentityNumber = string.Empty;
    }

    /// <summary>
    /// Creates a new Person with the specified required fields.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="email">The email address.</param>
    /// <param name="identityNumber">The identity number.</param>
    /// <param name="birthDate">The birth date (must be a past date).</param>
    /// <param name="phone">The optional phone number.</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone = null)
    {
        ValidateRequiredString(firstName, nameof(firstName));
        ValidateRequiredString(lastName, nameof(lastName));
        ValidateRequiredString(email, nameof(email));
        ValidateRequiredString(identityNumber, nameof(identityNumber));
        ValidateBirthDate(birthDate);

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Phone = phone;
    }

    private static void ValidateRequiredString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} is required and cannot be null, empty, or whitespace.", paramName);
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate >= DateTime.Today)
            throw new ArgumentException("BirthDate must be a past date.", nameof(birthDate));
    }
}
