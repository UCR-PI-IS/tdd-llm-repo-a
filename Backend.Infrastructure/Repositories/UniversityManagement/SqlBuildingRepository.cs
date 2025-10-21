using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.Locations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.UniversityManagement;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.UniversityManagement;

/// <summary>
/// SQL-based implementation of <see cref="IBuildingsRepository"/> for performing 
/// CRUD operations on <see cref="Building"/> entities using Entity Framework Core.
/// </summary>
internal class SqlBuildingRepository : IBuildingsRepository
{
    /// <summary>
    /// Repository for accessing area-related data.
    /// </summary>
    public readonly IAreaRepository _areaRepository;

    private readonly ThemeParkDataBaseContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlBuildingRepository"/> class.
    /// </summary>
    /// <param name="dbContext">Database context used for EF Core operations.</param>
    /// <param name="areaRepository">Area repository for related area operations.</param>
    public SqlBuildingRepository(ThemeParkDataBaseContext dbContext, IAreaRepository areaRepository)
    {
        _dbContext = dbContext;
        _areaRepository = areaRepository;
    }

    /// <summary>
    /// Adds a new building to the database.
    /// </summary>
    /// <param name="building">The building entity to add.</param>
    /// <returns>True if the operation was successful; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown if the building does not include an associated area.</exception>
    public async Task<bool> AddBuildingAsync(Building building)
    {


        var existing = await _dbContext.Buildings
            .FirstOrDefaultAsync(b => b.Name == building.Name);

        if (existing is not null)
        {
            throw new DuplicatedEntityException($"A building with the name '{building.Name.Name}' already exists.");
        }

        _dbContext.Buildings.Add(building);
        return await SaveChangesAsync();
    }


    /// <summary>
    /// Updates the specified building with new property values.
    /// </summary>
    /// <param name="building">The building entity with updated values.</param>
    /// <param name="buildingId">The Id of the building to update.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public async Task<bool> UpdateBuildingAsync(Building building, int buildingId)
    {

        var existing = await _dbContext.Buildings
            .FirstOrDefaultAsync(b => b.BuildingInternalId == buildingId);

        if (existing is null)
        {
            return false;
        }

        existing.Name = building.Name;

        if (building.BuildingCoordinates is not null)
            existing.BuildingCoordinates = building.BuildingCoordinates;

        if (building.Dimensions is not null)
            existing.Dimensions = building.Dimensions;

        if (building.Color is not null)
            existing.Color = building.Color;

        if (building.Area is not null) 
            existing.Area = building.Area;

            return await SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a building by its id, including related area, campus, and university information.
    /// </summary>
    /// <param id="buildingId">The id of the building to retrieve.</param>
    /// <returns>The matching building if found; otherwise, null.</returns>
    public async Task<Building?> DisplayBuildingAsync(int buildingId)
    {
        return await _dbContext.Buildings
            .Include(b => b.Area)
                .ThenInclude(a => a.Campus)
                    .ThenInclude(c => c.University)
            .FirstOrDefaultAsync(b => b.BuildingInternalId == buildingId);
    }

    /// <summary>
    /// Retrieves a list of all buildings, including related area, campus, and university information.
    /// </summary>
    /// <returns>An enumerable of all buildings in the database.</returns>
    public async Task<IEnumerable<Building>> ListBuildingAsync()
    {
        return await _dbContext.Buildings
            .Include(b => b.Area)
                .ThenInclude(a => a.Campus)
                    .ThenInclude(c => c.University)
            .ToListAsync();
    }

    /// <summary>
    /// Deletes a building by its Id.
    /// </summary>
    /// <param name="buildingId">The name of the building to delete.</param>
    /// <returns>True if the deletion was successful; otherwise, false.</returns>
    public async Task<bool> DeleteBuildingAsync(int buildingId)
    {

        var existing = await _dbContext.Buildings
            .FirstOrDefaultAsync(b => b.BuildingInternalId == buildingId);

        if (existing is null)
            return false;

        _dbContext.Buildings.Remove(existing);
        return await SaveChangesAsync();
    }

    /// <summary>
    /// Commits changes to the database.
    /// </summary>
    /// <returns>True if the save operation succeeded; otherwise, false.</returns>
    private async Task<bool> SaveChangesAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
