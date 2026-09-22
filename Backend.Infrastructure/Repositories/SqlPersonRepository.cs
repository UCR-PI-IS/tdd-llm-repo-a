using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IPersonRepository"/>.
/// </summary>
internal class SqlPersonRepository : IPersonRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlPersonRepository"/> class.
    /// </summary>
    public SqlPersonRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new person to the database.
    /// </summary>
    public Task AddAsync(Person person)
    {
        return SqlPersonCommandHelper.AddAsync(_dbContext, person);
    }

    /// <summary>
    /// Checks whether a person with the specified email exists.
    /// </summary>
    public Task<bool> ExistsByEmailAsync(string email)
    {
        return SqlPersonQueryHelper.ExistsByEmailAsync(_dbContext, email);
    }

    /// <summary>
    /// Checks whether a person with the specified identity number exists.
    /// </summary>
    public Task<bool> ExistsByIdentityNumberAsync(string identityNumber)
    {
        return SqlPersonQueryHelper.ExistsByIdentityNumberAsync(_dbContext, identityNumber);
    }
}
