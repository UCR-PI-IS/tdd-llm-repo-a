using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for person operations.
/// </summary>
public class PersonService : IPersonService
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
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A service result containing the created person or an error message.</returns>
    public async Task<ServiceResult<Person>> CreatePersonAsync(Person person)
    {
        try
        {
            // Always check both email and identity number to satisfy test expectations
            var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
            var identityNumberExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists)
            {
                return ServiceResult<Person>.Failure("A person with this email already exists.");
            }

            if (identityNumberExists)
            {
                return ServiceResult<Person>.Failure("A person with this identity number already exists.");
            }

            // Add the person to the repository
            await _personRepository.AddAsync(person);

            return ServiceResult<Person>.Success(person);
        }
        catch (Exception ex)
        {
            return ServiceResult<Person>.Failure(ex.Message);
        }
    }
}
