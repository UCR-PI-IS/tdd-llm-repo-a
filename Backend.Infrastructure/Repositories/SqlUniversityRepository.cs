using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL-based implementation of <see cref="IUniversityRepository"/>.
/// Provides university data access operations in the database.
/// </summary>
internal class SqlUniversityRepository : IUniversityRepository
{
    private readonly UCRDatabaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlUniversityRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data access.</param>
    public SqlUniversityRepository(UCRDatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Adds a new university to the database and persists it.
    /// </summary>
    /// <param name="university">The university entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(University university)
    {
        await _dbContext.Universities.AddAsync(university);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a university with the specified name already exists.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if a university with the name exists, otherwise false.</returns>
    public Task<bool> ExistsByNameAsync(string name)
    {
        return Task.FromResult(_dbContext.Universities.Any(u => u.Name == name));
    }
}
