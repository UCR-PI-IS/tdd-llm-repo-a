using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.Spaces;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories.Spaces;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.Spaces;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories.Spaces;

internal class SqlFloorRepository : IFloorRepository
{
    /// <summary>
    /// Database context for accessing the theme park database.
    /// </summary>
    private readonly ThemeParkDataBaseContext _dbContext;

    /// <summary>
    /// Object for display logging information and errors.
    /// </summary>
    private readonly ILogger<SqlFloorRepository> _logger;


    /// <summary>
    /// Initializes a new instance of the <see cref="SqlFloorRepository"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used to access the theme park database.</param>
    public SqlFloorRepository(ThemeParkDataBaseContext dbContext, ILogger<SqlFloorRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new floor in a building.
    /// </summary>
    /// <param name="buildingId">The identifier of the building where the floor will be created.</param>
    /// <returns>
    /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
    /// Returns true if the floor was successfully created; otherwise, false.
    /// </returns>
    public async Task<bool> CreateFloorAsync(int buildingId)
    {
        var message = string.Empty;

        var building = await _dbContext.Buildings.FirstOrDefaultAsync(b => b.BuildingInternalId == buildingId);
        if (building is null)
        {
            message = $"Building with ID '{buildingId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        // Check the last floor number in the building
        var maxFloorNumber = _dbContext.Floors
            .Where(f => EF.Property<int>(f, "BuildingId") == building.BuildingInternalId)
            .AsEnumerable()
            .Select(f => f.Number.Value)
            .DefaultIfEmpty(0)
            .Max();

        var newFloorNumber = maxFloorNumber + 1;

        // Create the new floor
        var floor = new Floor(FloorNumber.Create(newFloorNumber));

        try
        {
            // Set the shadow property for the building ID
            _dbContext.Entry(floor).Property("BuildingId").CurrentValue = building.BuildingInternalId;

            // add floor to the database
            await _dbContext.Floors.AddAsync(floor);

            // Attempt to save changes
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Floor number '{newFloorNumber}' created in building '{buildingId}'.");
            return true;

        }
        catch (DbUpdateConcurrencyException ex)
        {
            message = $"Concurrency conflict creating the new floor in the building '{buildingId}'. It may have been updated or deleted by another user.";
            _logger.LogWarning(ex, message);
            throw new ConcurrencyConflictException(message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 547)
        {
            message = $"Unable to create floor. The specified buildingId '{buildingId}' does not exist.";
            _logger.LogWarning(ex, message);
            throw new NotFoundException(message);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            throw new DuplicatedEntityException($"A floor with number '{newFloorNumber}' already exists in building '{buildingId}'");
        }
    }

    /// <summary>
    /// Lists all floors available in a building.
    /// </summary>
    /// <param name="buildingId">The identifier of the building.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a list of <see cref="Floor"/> instances on the specified building.
    /// </returns>
    public async Task<List<Floor>?> ListFloorsAsync(int buildingId)
    {
        var message = string.Empty;

        try
        {
            var buildingExists = await _dbContext.Buildings.AnyAsync(b => b.BuildingInternalId == buildingId);

            if (!buildingExists)
            {
                message = $"Building with id {buildingId} does not exist.";
                _logger.LogInformation(message);
                throw new NotFoundException(message); 
            }

            var floors = await _dbContext.Floors.Where(f =>
                EF.Property<int>(f, "BuildingId") == buildingId).ToListAsync();


            return floors;
        }
        catch (DbUpdateException ex)
        {
            message = $"Database update error while listing floors for buildingId {buildingId}.";
            _logger.LogWarning(ex, message);
            throw new DomainException(message);
        }
    }

    /// <summary>
    /// Deletes a specific floor in a building.
    /// </summary>
    /// <param name="floorId">The identifier of the floor that will be removed</param>
    /// <returns>
    /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
    /// Returns true if the floor was successfully created; otherwise, false.
    /// </returns>
    public async Task<bool> DeleteFloorAsync(int floorId)
    {
        var message = string.Empty;

        // Search the floor by its ID
        var floor = await _dbContext.Floors
                .FirstOrDefaultAsync(f => f.FloorId == floorId);

        if (floor is null)
        {
            message = $"Floor with id '{floorId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        // Get the building ID from the shadow property
        var buildingId = _dbContext.Entry(floor).Property<int>("BuildingId").CurrentValue;
        // Search the building by its ID
        var building = await _dbContext.Buildings
            .FirstOrDefaultAsync(b => b.BuildingInternalId == buildingId);

        if (building is null)
        {
            message = $"Building reference for floor '{floorId}' not found.";
            _logger.LogWarning(message);
            throw new NotFoundException(message);
        }

        try
        {
            // Delete the floor
            _dbContext.Floors.Remove(floor);
            await _dbContext.SaveChangesAsync();

            // Get the ramaining floors in the building to reassign their numbers
            var remainingFloors = await _dbContext.Floors
                .Where(f => EF.Property<int>(f, "BuildingId") == buildingId)
                .OrderBy(f => f.Number)
                .ToListAsync();

            // Reassign the floor numbers for the remaining floors
            for (int i = 0; i < remainingFloors.Count; ++i)
            {
                remainingFloors[i].ChangeFloorNumber(i + 1);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Floor with ID '{floorId}' successfully deleted.");
            return true;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            message = $"Concurrency conflict deleting the floor '{floorId}' of building. It may have been updated or deleted by another user.";
            _logger.LogWarning(ex, message);
            throw new ConcurrencyConflictException(message);
        }
    }


}