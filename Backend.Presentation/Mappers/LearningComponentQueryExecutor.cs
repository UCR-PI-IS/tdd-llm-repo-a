using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Executes the learning-component list query and maps outcomes to HTTP results.
/// </summary>
internal static class LearningComponentQueryExecutor
{
    /// <summary>
    /// Loads components for the given learning space and returns OK or NotFound.
    /// </summary>
    /// <param name="learningComponentService">Service used to load components.</param>
    /// <param name="learningSpaceId">Learning space identifier (already validated).</param>
    public static Task<IResult> ExecuteAsync(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        try
        {
            return Task.FromResult(
                LearningComponentOkResult.Create(learningComponentService, learningSpaceId));
        }
        catch (KeyNotFoundException ex)
        {
            return Task.FromResult(LearningComponentNotFoundResult.Create(ex.Message));
        }
    }
}
