using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds HTTP results for learning component listing endpoints.
/// </summary>
internal static class LearningComponentHttpResults
{
    private const string EmptyLearningSpaceIdMessage = "Learning space ID cannot be null or empty";

    public static IResult BadRequestEmptyLearningSpaceId()
    {
        return TypedResults.BadRequest(new ErrorResponse(EmptyLearningSpaceIdMessage));
    }

    public static IResult NotFound(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }

    public static IResult Ok(IReadOnlyList<LearningComponent> components)
    {
        return TypedResults.Ok(LearningComponentMapper.ToResponse(components));
    }
}
