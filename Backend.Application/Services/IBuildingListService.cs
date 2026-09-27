using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that retrieves building listings.
/// </summary>
public interface IBuildingListService
{
    /// <summary>
    /// Retrieves all buildings from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all <see cref="Building"/> entities.</returns>
    Task<List<Building>> GetAllBuildingsAsync();
}
