using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that manages whiteboards.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard with the specified parameters.
    /// </summary>
    /// <param name="request">The creation request containing all whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request);
}

/// <summary>
/// Request object for creating a whiteboard.
/// </summary>
public record CreateWhiteboardRequest(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);
