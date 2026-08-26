using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// A strongly-typed result wrapper for learning space creation that exposes the underlying <see cref="IResult"/>
/// for testability while avoiding explicit coupling to multiple framework union types in the handler.
/// </summary>
public class CreateLearningSpaceResult : IResult
{
    private readonly IResult _result;

    /// <summary>
    /// Gets the underlying result instance for test assertions.
    /// </summary>
    public IResult Result => _result;

    private CreateLearningSpaceResult(IResult result)
    {
        _result = result;
    }

    /// <summary>
    /// Creates a successful 201 Created result.
    /// </summary>
    public static CreateLearningSpaceResult Success<TResponse>(int learningSpaceId, TResponse response)
    {
        return new CreateLearningSpaceResult(
            TypedResults.Created($"/api/learningspaces/{learningSpaceId}", response));
    }

    /// <summary>
    /// Creates a 400 Bad Request result with the given message.
    /// </summary>
    public static CreateLearningSpaceResult BadRequest(string message)
    {
        return new CreateLearningSpaceResult(TypedResults.BadRequest(message));
    }

    /// <summary>
    /// Creates a 500 Problem result with the given detail.
    /// </summary>
    public static CreateLearningSpaceResult Problem(string detail)
    {
        return new CreateLearningSpaceResult(TypedResults.Problem(detail));
    }

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }
}
