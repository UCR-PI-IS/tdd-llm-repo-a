using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Builds error HTTP results for the learning-components list endpoint.
/// </summary>
internal static class LearningComponentsErrorResults
{
    private const string EmptyIdMessage = "Learning space ID cannot be null or empty.";

    public static IResult BadRequestEmptyId()
    {
        return TypedResults.BadRequest(new ErrorResponse(EmptyIdMessage));
    }

    public static IResult NotFound(string message)
    {
        return TypedResults.NotFound(new ErrorResponse(message));
    }
}
