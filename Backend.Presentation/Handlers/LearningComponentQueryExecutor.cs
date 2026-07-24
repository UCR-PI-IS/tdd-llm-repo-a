using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Executes the learning-components query and maps outcomes to HTTP results.
/// </summary>
internal static class LearningComponentQueryExecutor
{
    public static async Task<IResult> ExecuteAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        try
        {
            var components = await learningComponentService
                .GetComponentsByLearningSpaceIdAsync(learningSpaceId);

            return LearningComponentHttpResults.Ok(components);
        }
        catch (KeyNotFoundException ex)
        {
            return LearningComponentHttpResults.NotFound(ex.Message);
        }
    }
}
