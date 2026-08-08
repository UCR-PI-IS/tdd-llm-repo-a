using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response containing the list of components,
    /// a <see cref="BadRequest{T}"/> if the learning space ID is invalid,
    /// or a <see cref="NotFound{T}"/> if the learning space does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrWhiteSpace(learningSpaceId))
        {
            return TypedResults.BadRequest(new ErrorResponse("Learning space ID cannot be null or empty."));
        }

        try
        {
            var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            return TypedResults.Ok(GetLearningComponentsResponse.FromDomain(components));
        }
        catch (KeyNotFoundException ex)
        {
            return TypedResults.NotFound(new ErrorResponse(ex.Message));
        }
    }
}
