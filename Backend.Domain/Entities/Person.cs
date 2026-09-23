namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the Theme Park system.
/// </summary>
public class Person
{
    /// <summary>
    /// Unique identifier for the person.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// First name of the person.
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// Last name of the person.
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// Email address of the person.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Identity number of the person.
    /// </summary>
    public string IdentityNumber { get; }

    /// <summary>
    /// Birth date of the person. Must be a past date.
    /// </summary>
    public DateTime BirthDate { get; }

    /// <summary>
    /// Optional phone number of the person.
    /// </summary>
    public string? Phone { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Person"/> class.
    /// </summary>
    /// <param name="id">Unique identifier.</param>
    /// <param name="firstName">First name. Required.</param>
    /// <param name="lastName">Last name. Required.</param>
    /// <param name="email">Email address. Required.</param>
    /// <param name="identityNumber">Identity number. Required.</param>
    /// <param name="birthDate">Birth date. Must be a past date.</param>
    /// <param name="phone">Optional phone number.</param>
    /// <exception cref="ArgumentException">Thrown when required fields are null, empty, or whitespace, or when birthDate is not a past date.</exception>
    public Person(int id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone = null)
    {
        ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));
        ThrowIfNullOrWhiteSpace(email, nameof(email));
        ThrowIfNullOrWhiteSpace(identityNumber, nameof(identityNumber));
        ThrowIfNotPastDate(birthDate, nameof(birthDate));

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Phone = phone;
    }

    private static void ThrowIfNullOrWhiteSpace(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
    }

    private static void ThrowIfNotPastDate(DateTime value, string paramName)
    {
        if (value >= DateTime.Today)
            throw new ArgumentException("Birth date must be a past date.", paramName);
    }
}
