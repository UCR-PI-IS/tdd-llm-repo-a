using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to retrieve learning components for a given learning space ID.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An <see cref="IResult"/> that is either <see cref="Ok{T}"/> with the list of components,
    /// <see cref="BadRequest{T}"/> for invalid input, or <see cref="NotFound{T}"/> when the learning space does not exist.
    /// </returns>
    public static Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        return HandlerResultHelper.ExecuteAsync(async () =>
        {
            var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var response = LearningComponentMapper.ToResponse(components);
            return TypedResults.Ok(response);
        });
    }
}
