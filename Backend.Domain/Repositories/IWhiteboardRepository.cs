using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for persisting whiteboard entities.
/// </summary>
public interface IWhiteboardRepository
{
    /// <summary>
    /// Adds a new whiteboard to the data source.
    /// </summary>
    /// <param name="whiteboard">The whiteboard entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(Whiteboard whiteboard);
}
