using Microsoft.AspNetCore.Http;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

public readonly struct CreatePersonHandlerResult : IResult
{
    private readonly IResult _result;

    public CreatePersonHandlerResult(IResult result)
    {
        _result = result;
    }

    public IResult Result => _result;

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }
}
