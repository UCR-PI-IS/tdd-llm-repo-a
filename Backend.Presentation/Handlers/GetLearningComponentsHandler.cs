using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to retrieve learning components for a specific learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response with the list of components on success,
    /// a <see cref="BadRequest{T}"/> response when the learning space ID is invalid,
    /// or a <see cref="NotFound{T}"/> response when the learning space does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        try
        {
            var response = await LearningComponentResponseBuilder.BuildAsync(
                learningComponentService, learningSpaceId);
            return TypedResults.Ok(response);
        }
        catch (ArgumentException ex)
        {
            return HandlerErrorMapper.ToBadRequest(ex);
        }
        catch (KeyNotFoundException ex)
        {
            return HandlerErrorMapper.ToNotFound(ex);
        }
    }
}
