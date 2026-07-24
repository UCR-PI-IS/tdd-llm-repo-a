using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds HTTP results for the get-learning-components endpoint.
/// </summary>
internal static class LearningComponentHttpResults
{
    public static IResult BadRequest(string message) =>
        TypedResults.BadRequest(new ErrorResponse(message));

    public static IResult NotFound(string message) =>
        TypedResults.NotFound(new ErrorResponse(message));

    public static IResult Ok(IEnumerable<LearningComponent> components) =>
        TypedResults.Ok(LearningComponentMapper.ToResponse(components));
}
