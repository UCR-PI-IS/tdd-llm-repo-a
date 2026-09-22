using Microsoft.AspNetCore.Http;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Wrapper for person creation handler results that implements <see cref="IResult"/>
/// to remain compatible with Minimal APIs while reducing class coupling.
/// </summary>
public readonly struct PersonCreationResult : IResult
{
    private readonly IResult _result;

    /// <summary>
    /// Gets the underlying result.
    /// </summary>
    public IResult Result => _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonCreationResult"/> struct.
    /// </summary>
    /// <param name="result">The underlying ASP.NET Core result.</param>
    public PersonCreationResult(IResult result)
    {
        _result = result;
    }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }
}
