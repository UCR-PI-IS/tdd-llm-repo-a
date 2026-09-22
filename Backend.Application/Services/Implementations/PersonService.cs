using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for creating persons.
/// </summary>
internal class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonService"/> class.
    /// </summary>
    /// <param name="repository">The person repository dependency.</param>
    public PersonService(IPersonRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Creates a new person after checking for duplicates.
    /// </summary>
    public async Task<CreatePersonResult> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _repository.ExistsByEmailAsync(person.Email);
            var identityExists = await _repository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists)
                return CreatePersonResult.Failure("A person with this email already exists.");

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
