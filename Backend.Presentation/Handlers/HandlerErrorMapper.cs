using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Provides error-to-HTTP-response mapping for handler classes.
/// </summary>
internal static class HandlerErrorMapper
{
    /// <summary>
    /// Maps an <see cref="ArgumentException"/> to a <see cref="BadRequest{T}"/> response.
    /// </summary>
    /// <param name="ex">The argument exception to map.</param>
    /// <returns>A BadRequest result containing the error message.</returns>
    public static IResult ToBadRequest(ArgumentException ex)
        => TypedResults.BadRequest(new ErrorResponse(ex.Message));

    /// <summary>
    /// Maps a <see cref="KeyNotFoundException"/> to a <see cref="NotFound{T}"/> response.
    /// </summary>
    /// <param name="ex">The key-not-found exception to map.</param>
    /// <returns>A NotFound result containing the error message.</returns>
    public static IResult ToNotFound(KeyNotFoundException ex)
        => TypedResults.NotFound(new ErrorResponse(ex.Message));
}
