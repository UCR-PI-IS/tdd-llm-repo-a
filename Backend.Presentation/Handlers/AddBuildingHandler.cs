using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for adding a new building.
/// </summary>
public class AddBuildingHandler
{
    private readonly IBuildingService _buildingService;

    /// <summary>
    /// Initializes a new instance of the AddBuildingHandler class.
    /// </summary>
    /// <param name="buildingService">The building service.</param>
    public AddBuildingHandler(IBuildingService buildingService)
    {
        _buildingService = buildingService;
    }

    /// <summary>
    /// Handles the add building request.
    /// </summary>
    /// <param name="request">The add building request.</param>
    /// <returns>A response indicating the result of the operation.</returns>
    public async Task<AddBuildingResponse> HandleAsync(AddBuildingRequest request)
    {
        try
        {
            var result = await _buildingService.AddBuildingAsync(
                request.Name!,
                request.Color!,
                request.Height,
                request.Length,
                request.Width,
                request.X,
                request.Y,
                request.Z
            );

            return new AddBuildingResponse
            {
                Success = true,
                ErrorMessage = null
            };
        }
        catch (InvalidOperationException ex)
        {
            return new AddBuildingResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new AddBuildingResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
