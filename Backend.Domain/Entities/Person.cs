namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a person in the Theme Park system.
/// </summary>
public class Person
{
    /// <summary>
    /// Unique identifier for the person.
    /// </summary>
    public Guid Id { get; }

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
    /// Birth date of the person.
    /// </summary>
    public DateTime BirthDate { get; }

    /// <summary>
    /// Phone number of the person (optional).
    /// </summary>
    public string? Phone { get; }

    /// <summary>
    /// Initializes a new instance of the Person class with validation.
    /// </summary>
    public Person(Guid id, string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(identityNumber))
            throw new ArgumentException("IdentityNumber is required", nameof(identityNumber));

        if (birthDate > DateTime.Now)
            throw new ArgumentException("BirthDate cannot be in the future", nameof(birthDate));

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Phone = phone;
    }
}
