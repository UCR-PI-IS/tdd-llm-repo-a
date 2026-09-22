using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal static class CreatePersonResponses
{
    internal static IResult ValidationError() => TypedResults.BadRequest(new ValidationErrorResponse("Validation failed."));
    internal static IResult Conflict(string message) => TypedResults.Conflict(new ErrorResponse(message));
    internal static IResult Success() => TypedResults.Ok(new CreatePersonResponse(true));
}
