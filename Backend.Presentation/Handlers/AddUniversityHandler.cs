using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new university to the system.
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
    /// An <see cref="Ok{T}"/> response with the success message,
    /// or a <see cref="BadRequest{T}"/> if validation or duplicate check fails.
    /// </returns>
    public async Task<Results<Ok<AddUniversityResponse>, BadRequest<string>>> HandleAsync(string name, string country)
    {
        var result = await _service.AddUniversityAsync(name, country);

        if (result.IsSuccess)
        {
            return TypedResults.Ok(new AddUniversityResponse("University added successfully"));
        }

        return TypedResults.BadRequest(result.ErrorMessage ?? "An error occurred");
    }
}
