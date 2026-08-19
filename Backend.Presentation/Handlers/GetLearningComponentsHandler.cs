using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components for a given learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning component data.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>
    /// An <see cref="Ok{T}"/> response containing the list of learning components,
    /// a <see cref="BadRequest{T}"/> if the learning space ID is invalid,
    /// or a <see cref="NotFound{T}"/> if the learning space does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string? learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            return LearningComponentResponseFactory.CreateBadRequestResponse();

        try
        {
            var components = await learningComponentService
                .GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            return LearningComponentResponseFactory.CreateOkResponse(components);
        }
        catch (KeyNotFoundException ex)
        {
            return LearningComponentResponseFactory.CreateNotFoundResponse(ex.Message);
        }
    }
}
