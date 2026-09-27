using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages building listing operations.
/// </summary>
public interface IBuildingListService
{
    /// <summary>
    /// Retrieves a list of all buildings available in the database.
    /// </summary>
    /// <returns>A list of building entities.</returns>
    Task<List<Building>> GetAllBuildingsAsync();
}
