using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Handler for getting learning components by learning space ID.
/// </summary>
public class GetLearningComponentsHandler
{
    private readonly ILearningComponentService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetLearningComponentsHandler"/> class.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    public GetLearningComponentsHandler(ILearningComponentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Handles the request to get learning components for a specific learning space.
    /// </summary>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An HTTP result containing the components or an error.</returns>
    public async Task<Results<Ok<GetLearningComponentsResponse>, BadRequest<string>>> HandleAsync(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return TypedResults.BadRequest("Learning space ID cannot be null or empty.");
        }

        var components = await _service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        var response = new GetLearningComponentsResponse
        {
            Components = components.Select(c => new LearningComponentDto
            {
                ComponentId = c.ComponentId,
                LearningSpaceId = c.LearningSpaceId,
                Width = c.Width,
                Height = c.Height,
                Depth = c.Depth,
                X = c.X,
                Y = c.Y,
                Z = c.Z,
                Orientation = c.Orientation
            }).ToList()
        };

        return TypedResults.Ok(response);
    }
}
