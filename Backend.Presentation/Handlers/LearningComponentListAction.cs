using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using Microsoft.AspNetCore.Http;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Orchestrates listing learning components and mapping outcomes to HTTP results.
/// </summary>
internal static class LearningComponentListAction
{
    public static async Task<IResult> ExecuteAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return LearningComponentHttpResults.BadRequestEmptyLearningSpaceId();
        }

        try
        {
            var components = await learningComponentService.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            return LearningComponentHttpResults.Ok(components);
        }
        catch (KeyNotFoundException ex)
        {
            return LearningComponentHttpResults.NotFound(ex.Message);
        }
    }
}
