namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that manages whiteboard operations including updates.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Updates an existing whiteboard with the specified parameters.
    /// </summary>
    /// <param name="dto">The update data transfer object containing the new values.</param>
    /// <returns>A result indicating success or failure of the update operation.</returns>
    Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto);
}
