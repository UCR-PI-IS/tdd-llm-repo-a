using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing buildings in the database.
/// </summary>
internal class BuildingRepository : IBuildingRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the BuildingRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BuildingRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<bool> AddAsync(Building building)
    {
        await _context.Buildings.AddAsync(building);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<Building?> GetByNameAsync(string name)
    {
        return await _context.Buildings
            .FirstOrDefaultAsync(b => b.Name == name);
    }
}
