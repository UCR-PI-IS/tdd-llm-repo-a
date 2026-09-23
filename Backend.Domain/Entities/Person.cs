namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the Theme Park system.
/// </summary>
public class Person
{
    /// <summary>
    /// Unique identifier for the person.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// First name of the person.
    /// </summary>
    public string FirstName { get; private set; }

    /// <summary>
    /// Last name of the person.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Email address of the person.
    /// </summary>
    public string Email { get; private set; }

    /// <summary>
    /// Identity number of the person.
    /// </summary>
    public string IdentityNumber { get; private set; }

    /// <summary>
    /// Birth date of the person.
    /// </summary>
    public DateTime BirthDate { get; private set; }

    /// <summary>
    /// Optional phone number of the person.
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
    /// <param name="id">The person identifier.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="email">The email address.</param>
    /// <param name="identityNumber">The identity number.</param>
    /// <param name="birthDate">The birth date (must be a past date).</param>
    /// <param name="phone">Optional phone number.</param>
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
        if (value == null)
            throw new ArgumentException($"{paramName} is required.", paramName);

        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} is required.", paramName);
    }

    private static void ValidateBirthDate(DateTime birthDate)
    {
        if (birthDate.Date >= DateTime.Now.Date)
            throw new ArgumentException("BirthDate must be a past date.", nameof(birthDate));
    }
}
