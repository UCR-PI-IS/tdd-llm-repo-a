using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating persons.
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
    /// Creates a new person after checking for duplicate email and identity number.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A result indicating success or failure of the creation.</returns>
    public async Task<PersonCreationResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
            if (emailExists)
                return PersonCreationResult.Failure("A person with this email already exists.");

            var identityExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);
            if (identityExists)
                return PersonCreationResult.Failure("A person with this identity number already exists.");

            await _personRepository.AddAsync(person);
            return PersonCreationResult.Success();
        }
        catch (Exception ex)
        {
            return PersonCreationResult.Failure(ex.Message);
        }
    }
}
