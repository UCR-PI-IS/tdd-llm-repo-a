using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating persons.
/// </summary>
internal class PersonService : IPersonCreateService
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
    /// <param name="person">The person to create.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    public async Task<CreatePersonResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
            var identityNumberExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists && identityNumberExists)
            {
                return CreatePersonResult.Failure("A person with this email and identity number already exists.");
            }

            if (emailExists)
            {
                return CreatePersonResult.Failure("A person with this email already exists.");
            }

            if (identityNumberExists)
            {
                return CreatePersonResult.Failure("A person with this identity number already exists.");
            }

            await _personRepository.AddAsync(person);
            return CreatePersonResult.Success();
        }
        catch (Exception ex)
        {
            return CreatePersonResult.Failure(ex.Message);
        }
    }
}
