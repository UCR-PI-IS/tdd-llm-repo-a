using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

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
    /// <returns>An <see cref="AddUniversityResult"/> wrapping the result.</returns>
    public async Task<AddUniversityResult> HandleAsync(string name, string country)
    {
        var result = await _service.AddUniversityAsync(name, country);

        if (!result.IsSuccess)
        {
            return AddUniversityResult.Failure(result.ErrorMessage!);
        }

        return AddUniversityResult.Success("University added successfully");
    }
}
