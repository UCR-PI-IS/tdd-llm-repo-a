using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IPersonRepository"/>.
/// Provides person data access operations in the database.
/// </summary>
internal class SqlPersonRepository : IPersonRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlPersonRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlPersonRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new person to the database and persists it.
    /// </summary>
    /// <param name="person">The person entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(Person person)
    {
        await _dbContext.Persons.AddAsync(person);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a person with the specified email already exists.
    /// </summary>
    /// <param name="email">The email to check.</param>
    /// <returns>True if a person with the email exists, otherwise false.</returns>
    public Task<bool> ExistsByEmailAsync(string email)
    {
        return Task.FromResult(_dbContext.Persons.Any(p => p.Email == email));
    }

    /// <summary>
    /// Checks if a person with the specified identity number already exists.
    /// </summary>
    /// <param name="identityNumber">The identity number to check.</param>
    /// <returns>True if a person with the identity number exists, otherwise false.</returns>
    public Task<bool> ExistsByIdentityNumberAsync(string identityNumber)
    {
        return Task.FromResult(_dbContext.Persons.Any(p => p.IdentityNumber == identityNumber));
    }
}
