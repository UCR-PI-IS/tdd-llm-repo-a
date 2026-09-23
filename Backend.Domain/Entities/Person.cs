namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the Theme Park system.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets the unique identifier for the person.
    /// </summary>
    public int Id { get; }

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
    /// Initializes a new instance of the <see cref="Person"/> class with required fields and an optional phone number.
    /// </summary>
    /// <param name="id">The unique identifier for the person.</param>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>
    /// <param name="email">The email address of the person.</param>
    /// <param name="identityNumber">The identity number of the person.</param>
    /// <param name="birthDate">The birth date of the person. Must be a past date.</param>
    /// <param name="phone">The optional phone number of the person.</param>
    /// <exception cref="ArgumentException">Thrown when any required string field is null, empty, or whitespace, or when birthDate is not a past date.</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null, empty, or whitespace.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null, empty, or whitespace.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null, empty, or whitespace.", nameof(email));
        if (string.IsNullOrWhiteSpace(identityNumber))
            throw new ArgumentException("Identity number cannot be null, empty, or whitespace.", nameof(identityNumber));
        if (birthDate >= DateTime.Today)
            throw new ArgumentException("Birth date must be a past date.", nameof(birthDate));

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Phone = phone;
    }
}
