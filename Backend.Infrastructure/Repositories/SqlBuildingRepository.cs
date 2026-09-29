using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// SQL Server implementation of the building repository.
/// </summary>
internal class SqlBuildingRepository : IBuildingRepository
{
    private readonly UCRDatabaseContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBuildingRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SqlBuildingRepository(UCRDatabaseContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building to add.</param>
    /// <returns>The added building.</returns>
    public async Task<Building> AddAsync(Building building)
    {
        await _context.Buildings.AddAsync(building);
        await _context.SaveChangesAsync();
        return building;
    }

    /// <summary>
    /// Checks if a building with the specified name already exists.
    /// </summary>
    /// <param name="name">The building name to check.</param>
    /// <returns>True if a building with the name exists; otherwise, false.</returns>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Buildings.AnyAsync(b => b.Name == name);
    }
}
