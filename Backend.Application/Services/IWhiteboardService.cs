namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service interface for whiteboard operations.
/// </summary>
public interface IWhiteboardService
{
    /// <summary>
    /// Creates a new whiteboard in a learning space.
    /// </summary>
    /// <param name="whiteboardId">The whiteboard identifier</param>
    /// <param name="width">Width of the whiteboard in meters</param>
    /// <param name="height">Height of the whiteboard in meters</param>
    /// <param name="learningSpaceId">The learning space identifier</param>
    /// <returns>Result indicating success or failure</returns>
    Task<Result> CreateWhiteboardAsync(string whiteboardId, double width, double height, string learningSpaceId);
}
