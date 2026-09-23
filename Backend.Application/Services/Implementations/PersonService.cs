using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service for managing person creation with duplicate checking.
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
    /// Creates a new person after checking for duplicate email and identity number.
    /// </summary>
    /// <param name="person">The person to create.</param>
    /// <returns>A result indicating success or failure.</returns>
    public async Task<CreatePersonResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _repository.ExistsByEmailAsync(person.Email);
            if (emailExists)
                return CreatePersonResult.Failure("A person with this email already exists.");

            var identityExists = await _repository.ExistsByIdentityNumberAsync(person.IdentityNumber);
            if (identityExists)
                return CreatePersonResult.Failure("A person with this identity number already exists.");

            await _repository.AddAsync(person);
            return CreatePersonResult.Success();
        }
        catch (Exception ex)
        {
            return CreatePersonResult.Failure(ex.Message);
        }
    }
}
