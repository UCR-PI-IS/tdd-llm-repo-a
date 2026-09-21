using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for managing person data.
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
            var duplicateError = await CheckForDuplicatesAsync(person);
            if (duplicateError != null)
            {
                return OperationResult<Person>.Failure(duplicateError);
            }

            await _personRepository.AddAsync(person);
            return OperationResult<Person>.Success(person);
        }
        catch (Exception ex)
        {
            return OperationResult<Person>.Failure(ex.Message);
        }
    }

    private async Task<string?> CheckForDuplicatesAsync(Person person)
    {
        var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
        var identityExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);

        if (emailExists && identityExists)
        {
            return "A person with this email and identity number already exists.";
        }

        if (emailExists)
        {
            return "A person with this email already exists.";
        }

        if (identityExists)
        {
            return "A person with this identity number already exists.";
        }

        return null;
    }
}
