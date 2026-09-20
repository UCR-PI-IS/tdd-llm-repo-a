using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for whiteboard service operations including updates.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Updates an existing whiteboard with new data.
    /// </summary>
    /// <param name="dto">The update data transfer object.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto);
}
