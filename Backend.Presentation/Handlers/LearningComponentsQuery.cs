using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Executes the learning-components query and maps outcomes to HTTP results.
/// </summary>
internal static class LearningComponentsQuery
{
    public static async Task<IResult> ExecuteAsync(Func<Task<List<LearningComponent>>> fetch)
    {
        try
        {
            return LearningComponentsOkResult.Create(await fetch());
        }
        catch (KeyNotFoundException ex)
        {
            return LearningComponentsErrorResults.NotFound(ex.Message);
        }
    }
}
