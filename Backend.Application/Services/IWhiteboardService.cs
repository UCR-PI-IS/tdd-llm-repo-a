using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that updates whiteboards.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Updates an existing whiteboard with the specified parameters.
    /// </summary>
    /// <param name="dto">The update DTO containing whiteboard parameters.</param>
    /// <returns>A result indicating success or failure with details.</returns>
    Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto);
}
