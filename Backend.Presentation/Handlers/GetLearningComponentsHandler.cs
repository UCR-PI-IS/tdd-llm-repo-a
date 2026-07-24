using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching the list of learning components in a learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to list learning components for a learning space.
    /// </summary>
    /// <param name="learningComponentService">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The learning space identifier.</param>
    /// <returns>
    /// OK with the component list, BadRequest when the id is invalid, or NotFound when the space does not exist.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return LearningComponentErrorResults.MissingLearningSpaceId();
        }

        try
        {
            return await LearningComponentOkResults.FromComponentsAsync(
                learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId));
        }
        catch (KeyNotFoundException ex)
        {
            return LearningComponentErrorResults.NotFound(ex.Message);
        }
    }
}
