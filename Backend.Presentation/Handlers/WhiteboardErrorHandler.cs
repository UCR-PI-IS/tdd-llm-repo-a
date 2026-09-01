using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handles error mapping for whiteboard operations.
/// </summary>
internal static class WhiteboardErrorHandler
{
    /// <summary>
    /// Maps an exception to the appropriate HTTP result type.
    /// </summary>
    /// <param name="ex">The exception to handle.</param>
    /// <returns>An HTTP result representing the error.</returns>
    public static Results<Ok<CreateWhiteboardResponse>, BadRequest<string>, NotFound<string>, ProblemHttpResult> Handle(Exception ex)
    {
        return ex switch
        {
            ArgumentException => TypedResults.BadRequest(ex.Message),
            NotFoundException => TypedResults.NotFound(ex.Message),
            ValidationException => TypedResults.BadRequest(ex.Message),
            _ => TypedResults.Problem("An unexpected error occurred while creating the whiteboard.")
        };
    }
}
