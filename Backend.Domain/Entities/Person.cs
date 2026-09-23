namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the system.
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
    /// Gets the optional phone number of the person.
    /// </summary>
    public string? Phone { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class with required fields.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="email">The email address.</param>
    /// <param name="identityNumber">The identity number.</param>
    /// <param name="birthDate">The birth date.</param>
    /// <exception cref="ArgumentException">Thrown when any required field is null, empty, or whitespace, or when birthDate is not a past date.</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate)
        : this(id, firstName, lastName, email, identityNumber, birthDate, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class with all fields including optional phone.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <param name="email">The email address.</param>
    /// <param name="identityNumber">The identity number.</param>
    /// <param name="birthDate">The birth date.</param>
    /// <param name="phone">The optional phone number.</param>
    /// <exception cref="ArgumentException">Thrown when any required field is null, empty, or whitespace, or when birthDate is not a past date.</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone)
    {
        ValidateRequiredString(firstName, nameof(firstName));
        ValidateRequiredString(lastName, nameof(lastName));
        ValidateRequiredString(email, nameof(email));
        ValidateRequiredString(identityNumber, nameof(identityNumber));
        ValidatePastDate(birthDate, nameof(birthDate));

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
            throw new ArgumentException($"{paramName} cannot be null.", paramName);
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty or whitespace.", paramName);
    }

    private static void ValidatePastDate(DateTime value, string paramName)
    {
        if (value >= DateTime.Today)
            throw new ArgumentException("Birth date must be a past date.", paramName);
    }
}
