using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Wrapper for add university handler results that implements <see cref="IResult"/>
/// to remain compatible with Minimal APIs while reducing class coupling.
/// </summary>
public readonly struct AddUniversityResult : IResult
{
    private readonly IResult _result;

    /// <summary>
    /// Gets the underlying result.
    /// </summary>
    public IResult Result => _result;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddUniversityResult"/> struct.
    /// </summary>
    /// <param name="result">The underlying ASP.NET Core result.</param>
    public AddUniversityResult(IResult result)
    {
        _result = result;
    }

    /// <summary>
    /// Creates a success result with the specified message.
    /// </summary>
    /// <param name="message">The success message.</param>
    /// <returns>An <see cref="AddUniversityResult"/> wrapping an OK response.</returns>
    public static AddUniversityResult Success(string message)
    {
        return new AddUniversityResult(TypedResults.Ok(new AddUniversityResponse(message)));
    }

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>An <see cref="AddUniversityResult"/> wrapping a bad request response.</returns>
    public static AddUniversityResult Failure(string error)
    {
        return new AddUniversityResult(TypedResults.BadRequest(error));
    }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }
}
