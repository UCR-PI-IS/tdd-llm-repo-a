using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that manages whiteboard creation.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard in the specified learning space.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}
