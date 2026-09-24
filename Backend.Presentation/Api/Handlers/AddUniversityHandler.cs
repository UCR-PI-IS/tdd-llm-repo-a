using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

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
    /// An <see cref="AddUniversityResult"/> wrapping the result,
    /// either OK with success message or BadRequest with error message.
    /// </returns>
    public async Task<AddUniversityResult> HandleAsync(string name, string country)
    {
        var result = await _service.AddUniversityAsync(name, country);

        return !result.IsSuccess
            ? AddUniversityResult.Failure(result.ErrorMessage!)
            : AddUniversityResult.Success();
    }
}

/// <summary>
/// Represents the result of an add university operation.
/// </summary>
public class AddUniversityResult
{
    /// <summary>
    /// Gets the result of the operation.
    /// </summary>
    public Microsoft.AspNetCore.Http.IResult Result { get; }

    private AddUniversityResult(Microsoft.AspNetCore.Http.IResult result)
    {
        Result = result;
    }

    public static AddUniversityResult Success() =>
        new(TypedResults.Ok(new AddUniversityResponse { Message = "University added successfully" }));

    public static AddUniversityResult Failure(string error) =>
        new(TypedResults.BadRequest(error));
}
