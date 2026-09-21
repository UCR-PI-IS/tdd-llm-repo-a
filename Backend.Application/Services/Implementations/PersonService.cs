using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for person management operations.
/// </summary>
internal class PersonService : IPersonService
{
    private readonly IPersonRepository _personRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    /// <param name="personRepository">The person repository dependency.</param>
    public PersonService(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    /// <summary>
    /// Creates a new person in the system after checking for duplicates.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>An operation result containing the created person or an error message.</returns>
    public async Task<OperationResult<Person>> CreatePersonAsync(Person person)
    {
        try
        {
            // Check for duplicate email and identity number
            var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
            var identityExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists)
            {
                return Failure("A person with this email already exists.");
            }

            if (identityExists)
            {
                return Failure("A person with this identity number already exists.");
            }

            // Add the person to the repository
            await _personRepository.AddAsync(person);

            return Success(person);
        }
        catch (Exception ex)
        {
            return Failure(ex.Message);
        }
    }

    private static OperationResult<Person> Success(Person person)
    {
        return new OperationResult<Person>
        {
            IsSuccess = true,
            Value = person
        };
    }

    private static OperationResult<Person> Failure(string message)
    {
        return new OperationResult<Person>
        {
            IsSuccess = false,
            ErrorMessage = message
        };
    }
}
