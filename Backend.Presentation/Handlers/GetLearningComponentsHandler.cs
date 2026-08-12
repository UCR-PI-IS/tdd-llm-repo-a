using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentListService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An appropriate HTTP response containing the components, a bad request, or a not found result.</returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentListService learningComponentListService,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty"));
        }

        try
        {
            var components = await learningComponentListService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var dtos = components.Select(LearningComponentDto.FromDomain).ToList();
            return TypedResults.Ok(new GetLearningComponentsResponse(dtos));
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
