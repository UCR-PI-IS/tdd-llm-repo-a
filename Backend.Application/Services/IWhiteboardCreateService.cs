using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for the service that handles whiteboard creation.
/// </summary>
public interface IWhiteboardCreateService
{
    /// <summary>
    /// Creates a new whiteboard in the specified learning space.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard details.</param>
    /// <returns>The created whiteboard entity.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}
