using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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

    /// <inheritdoc />
    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }
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
    /// <param name="country">The country where the university is located.</param>
    /// <returns>
    /// An <see cref="AddUniversityResult"/> wrapping the response,
    /// a bad request if validation fails or the university already exists.
    /// </returns>
    public async Task<AddUniversityResult> HandleAsync(string name, string country)
    {
        var result = await _service.AddUniversityAsync(name, country);

        if (result.IsSuccess)
        {
            return new AddUniversityResult(
                TypedResults.Ok(new AddUniversityResponse("University added successfully")));
        }

        return new AddUniversityResult(
            TypedResults.BadRequest<string>(result.ErrorMessage!));
    }
}
