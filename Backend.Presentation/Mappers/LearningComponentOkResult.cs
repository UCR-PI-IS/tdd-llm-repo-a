using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Builds OK HTTP results after loading learning components.
/// </summary>
internal static class LearningComponentOkResult
{
    /// <summary>
    /// Loads components and returns an OK result.
    /// </summary>
    /// <param name="learningComponentService">Service used to load components.</param>
    /// <param name="learningSpaceId">Learning space identifier.</param>
    public static IResult Create(
        ILearningComponentService learningComponentService,
        string learningSpaceId)
    {
        var response = LearningComponentListLoader.Load(learningComponentService, learningSpaceId);
        return TypedResults.Ok(response);
    }
}
