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
    /// <param name="dto">The update request containing whiteboard parameters.</param>
    /// <returns>The result of the update operation.</returns>
    Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto);
}
