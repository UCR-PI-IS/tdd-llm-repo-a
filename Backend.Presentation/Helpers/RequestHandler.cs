using Microsoft.AspNetCore.Http;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Helpers;

internal static class RequestHandler
{
    public static async Task<IResult> ExecuteWithErrorResponseAsync<TInput, TOutput>(
        Func<Task<TInput>> action,
        Func<TInput, TOutput> successMapper)
    {
        try
        {
            var result = await action();
            return Results.Ok(successMapper(result));
        }
        catch (Exception ex)
        {
            var errorResult = ErrorResponseMapper.TryMapError(ex);
            if (errorResult is not null)
                return errorResult;
            throw;
        }
    }
}
