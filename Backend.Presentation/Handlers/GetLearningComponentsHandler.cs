using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching learning components for a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An <see cref="IResult"/> containing either the list of components or an error response.</returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var response = LearningComponentMapper.ToResponse(components);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex) when (ex.ParamName == "learningSpaceId")
        {
            return TypedResults.BadRequest(LearningComponentMapper.InvalidLearningSpaceIdError());
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(LearningComponentMapper.NotFoundError(ex.Message));
        }
    }
}
