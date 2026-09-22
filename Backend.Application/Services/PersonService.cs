using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service for managing person operations.
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
    /// Creates a new person in the system.
    /// </summary>
    /// <param name="person">The person entity to create.</param>
    /// <returns>A service result containing the created person or an error message.</returns>
    public async Task<ServiceResult<Person>> CreatePersonAsync(Person person)
    {
        try
        {
            var emailExists = await _repository.ExistsByEmailAsync(person.Email);
            var identityExists = await _repository.ExistsByIdentityNumberAsync(person.IdentityNumber);

            if (emailExists && identityExists)
                return Failure("A person with this email or identity number already exists.");

            if (emailExists)
                return Failure("A person with this email already exists.");

            if (identityExists)
                return Failure("A person with this identity number already exists.");

            await _repository.AddAsync(person);

            return new ServiceResult<Person> { IsSuccess = true, Data = person };
        }
        catch (Exception ex)
        {
            return Failure(ex.Message);
        }
    }

    private static ServiceResult<Person> Failure(string message) =>
        new ServiceResult<Person> { IsSuccess = false, ErrorMessage = message };
}
