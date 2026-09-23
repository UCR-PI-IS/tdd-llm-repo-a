using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service that orchestrates person creation, including duplicate checks.
/// </summary>
public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    /// <param name="repository">The person repository.</param>
    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates a new person after verifying no duplicate email or identity number exists.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    public async Task<PersonCreationResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _repository.ExistsByEmailAsync(person.Email);
            var identityNumberExists = await _repository.ExistsByIdentityNumberAsync(person.IdentityNumber);
            
            if (emailExists)
            {
                return new PersonCreationResult(false, "A person with this email already exists.");
            }

            if (identityNumberExists)
            {
                return new PersonCreationResult(false, "A person with this identity number already exists.");
            }

            await _repository.AddAsync(person);
            return new PersonCreationResult(true, null);
        }
        catch (Exception ex)
        {
            return new PersonCreationResult(false, ex.Message);
        }
    }
}
