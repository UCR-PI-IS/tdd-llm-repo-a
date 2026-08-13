using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Helpers;

internal static class ErrorResponseMapper
{
    public static IResult? TryMapError(Exception ex) => ex switch
    {
        ArgumentException => Results.BadRequest(new ErrorResponse(ex.Message)),
        KeyNotFoundException => Results.NotFound(new ErrorResponse(ex.Message)),
        _ => null
    };
}
