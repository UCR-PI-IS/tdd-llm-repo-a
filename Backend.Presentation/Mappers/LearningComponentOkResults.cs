using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds successful HTTP results for learning component list operations.
/// </summary>
internal static class LearningComponentOkResults
{
    /// <summary>
    /// Awaits domain components and maps them to a successful OK response.
    /// </summary>
    /// <param name="componentsTask">Task producing domain learning components.</param>
    public static async Task<IResult> FromComponentsAsync(Task<List<LearningComponent>> componentsTask)
    {
        var components = await componentsTask;
        return TypedResults.Ok(GetLearningComponentsResponse.FromEntities(components));
    }
}
