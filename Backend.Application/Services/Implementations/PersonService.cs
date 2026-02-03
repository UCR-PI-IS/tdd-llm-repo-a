using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for person operations.
/// </summary>
internal class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;

    /// <summary>
    /// Initializes a new instance of the PersonService class.
    /// </summary>
    /// <param name="personRepository">The person repository.</param>
    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    /// <inheritdoc/>
    public async Task<bool> CreatePersonAsync(string firstName, string lastName, string email, string identityNumber, DateTime birthDate, string? phone)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName is required", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName is required", nameof(lastName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        if (string.IsNullOrWhiteSpace(identityNumber))
            throw new ArgumentException("IdentityNumber is required", nameof(identityNumber));

        // Check for duplicate email
        var existingPersonByEmail = await _personRepository.GetByEmailAsync(email);
        if (existingPersonByEmail != null)
        {
            throw new InvalidOperationException("Person with this email already exists");
        }

        // Check for duplicate identity number
        var existingPersonByIdentityNumber = await _personRepository.GetByIdentityNumberAsync(identityNumber);
        if (existingPersonByIdentityNumber != null)
        {
            throw new InvalidOperationException("Person with this identity number already exists");
        }

        // Create new person
        var person = new Person(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            identityNumber,
            birthDate,
            phone
        );

        return await _personRepository.AddAsync(person);
    }
}
