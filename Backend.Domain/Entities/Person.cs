namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the ThemePark system.
/// </summary>
public class Person
{
    /// <summary>
    /// Unique identifier for the person.
    /// </summary>
    public int Id { get; private set; }

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
    /// Phone number of the person (optional).
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
    /// Constructor for creating a Person with required fields.
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email address</param>
    /// <param name="identityNumber">Identity number</param>
    /// <param name="birthDate">Birth date</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate)
        : this(id, firstName, lastName, email, identityNumber, birthDate, null)
    {
    }

    /// <summary>
    /// Constructor for creating a Person with all fields including optional Phone.
    /// </summary>
    /// <param name="id">Unique identifier</param>
    /// <param name="firstName">First name</param>
    /// <param name="lastName">Last name</param>
    /// <param name="email">Email address</param>
    /// <param name="identityNumber">Identity number</param>
    /// <param name="birthDate">Birth date</param>
    /// <param name="phone">Phone number (optional)</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone)
    {
        ValidateRequiredString(firstName, nameof(firstName));
        ValidateRequiredString(lastName, nameof(lastName));
        ValidateRequiredString(email, nameof(email));
        ValidateRequiredString(identityNumber, nameof(identityNumber));
        ValidateBirthDate(birthDate, nameof(birthDate));

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
        {
            throw new ArgumentException($"{paramName} cannot be null.", paramName);
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} cannot be empty or whitespace.", paramName);
        }
    }

    private static void ValidateBirthDate(DateTime birthDate, string paramName)
    {
        if (birthDate >= DateTime.Now.Date)
        {
            throw new ArgumentException("Birth date must be in the past.", paramName);
        }
    }
}
