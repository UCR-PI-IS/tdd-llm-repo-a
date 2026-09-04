using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that manages whiteboard operations.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard in the specified learning space.
    /// </summary>
    /// <param name="request">The request containing whiteboard creation parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space is not found.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard doesn't fit in the learning space.</exception>
    /// <exception cref="DatabaseException">Thrown when the database operation fails.</exception>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}
