using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for creating a whiteboard.
/// </summary>
public class CreateWhiteboardHandler
{
    private readonly IWhiteboardService _whiteboardService;

    /// <summary>
    /// Initializes a new instance of the CreateWhiteboardHandler class.
    /// </summary>
    /// <param name="whiteboardService">The whiteboard service</param>
    public CreateWhiteboardHandler(IWhiteboardService whiteboardService)
    {
        _whiteboardService = whiteboardService;
    }

    /// <summary>
    /// Handles the create whiteboard request.
    /// </summary>
    /// <param name="request">The create whiteboard request</param>
    /// <returns>A response with status code and result</returns>
    public async Task<(int StatusCode, CreateWhiteboardResponse Response)> HandleAsync(CreateWhiteboardRequest request)
    {
        var result = await _whiteboardService.CreateWhiteboardAsync(
            request.WhiteboardId,
            request.Width,
            request.Height,
            request.LearningSpaceId
        );

        if (result.IsSuccess)
        {
            return (201, new CreateWhiteboardResponse
            {
                Success = true
            });
        }

        return (400, new CreateWhiteboardResponse
        {
            Success = false,
            ErrorMessage = result.ErrorMessage
        });
    }
}
