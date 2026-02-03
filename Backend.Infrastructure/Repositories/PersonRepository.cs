using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing persons in the database.
/// </summary>
internal class PersonRepository : IPersonRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the PersonRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PersonRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<bool> AddAsync(Person person)
    {
        await _context.Persons.AddAsync(person);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<Person?> GetByEmailAsync(string email)
    {
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.Email == email);
    }

    /// <inheritdoc/>
    public async Task<Person?> GetByIdentityNumberAsync(string identityNumber)
    {
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.IdentityNumber == identityNumber);
    }
}
