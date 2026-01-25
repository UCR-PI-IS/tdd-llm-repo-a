using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for retrieving learning components by learning space ID
/// </summary>
public class GetLearningComponentsHandler
{
    private readonly ILearningComponentService _learningComponentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLearningComponentsHandler"/> class.
    /// </summary>
    /// <param name="learningComponentService">The learning component service dependency.</param>
    public GetLearningComponentsHandler(ILearningComponentService learningComponentService)
    {
        _learningComponentService = learningComponentService;
    }

    /// <summary>
    /// Handles the request to get learning components for a learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space</param>
    /// <returns>OK result with components or BadRequest if invalid input</returns>
    public async Task<Results<Ok<GetLearningComponentsResponse>, BadRequest<string>>> HandleAsync(string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            return TypedResults.BadRequest("Learning space ID cannot be null or empty.");
        }

        var components = await _learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        var response = new GetLearningComponentsResponse
        {
            Components = components.Select(c => new LearningComponentDto
            {
                ComponentId = c.ComponentId,
                LearningSpaceId = c.LearningSpaceId,
                ScaleX = c.ScaleX,
                ScaleY = c.ScaleY,
                ScaleZ = c.ScaleZ,
                PositionX = c.PositionX,
                PositionY = c.PositionY,
                PositionZ = c.PositionZ,
                Rotation = c.Rotation
            }).ToList()
        };

        return TypedResults.Ok(response);
    }
}
