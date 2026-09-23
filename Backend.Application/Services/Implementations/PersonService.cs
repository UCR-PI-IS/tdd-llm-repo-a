using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
    public async Task<CreatePersonResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _personRepository.ExistsByEmailAsync(person.Email);
            var identityExists = await _personRepository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists && identityExists)
                return new CreatePersonResult { IsSuccess = false, ErrorMessage = "A person with this email and identity number already exists." };
            if (emailExists)
                return new CreatePersonResult { IsSuccess = false, ErrorMessage = "A person with this email already exists." };
            if (identityExists)
                return new CreatePersonResult { IsSuccess = false, ErrorMessage = "A person with this identity number already exists." };

            await _personRepository.AddAsync(person);
            return new CreatePersonResult { IsSuccess = true };
        }
        catch (Exception ex)
        {
            return new CreatePersonResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }
}
