namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for the service that updates whiteboards.
/// </summary>
public interface IWhiteboardUpdateService
{
    /// <summary>
    /// Updates a whiteboard with the specified parameters.
    /// </summary>
    /// <param name="request">The update request containing whiteboard parameters.</param>
    /// <returns>The result of the update operation.</returns>
    Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(dynamic request);
}
