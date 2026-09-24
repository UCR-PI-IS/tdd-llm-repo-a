using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;

/// <summary>
/// Represents the result of an add-university handler operation.
/// Wraps the underlying <see cref="IResult"/> so callers can inspect the runtime type.
/// </summary>
public readonly record struct AddUniversityHandlerResult(IResult Result) : IResult
{
    /// <inheritdoc/>
    public Task ExecuteAsync(HttpContext httpContext) => Result.ExecuteAsync(httpContext);

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static AddUniversityHandlerResult Success(string message)
        => new(TypedResults.Ok(new AddUniversityResponse(message)));

    /// <summary>
    /// Creates a failure result.
    /// </summary>
    public static AddUniversityHandlerResult Failure(string error)
        => new(TypedResults.BadRequest(error));
}

/// <summary>
/// Handler for adding a new university.
/// </summary>
public class AddUniversityHandler
{
    private readonly IUniversityService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddUniversityHandler"/> class.
    /// </summary>
    /// <param name="service">The university service.</param>
    public AddUniversityHandler(IUniversityService service)
    {
        _service = service;
    }

    /// <summary>
    /// Handles the asynchronous request to add a new university.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country of the university.</param>
    /// <returns>
    /// An <see cref="AddUniversityHandlerResult"/> wrapping the success or failure response.
    /// </returns>
    public async Task<AddUniversityHandlerResult> HandleAsync(string name, string country)
    {
        var result = await _service.AddUniversityAsync(name, country);

        if (result.IsSuccess)
        {
            return AddUniversityHandlerResult.Success("University added successfully");
        }

        return AddUniversityHandlerResult.Failure(result.ErrorMessage!);
    }
}
