using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds the successful OK response for listing learning components.
/// </summary>
internal static class LearningComponentsOkResult
{
    public static IResult Create(IReadOnlyList<LearningComponent> components)
    {
        GetLearningComponentsResponse response = LearningComponentDtoFactory.ToResponse(components);
        return TypedResults.Ok(response);
    }
}
