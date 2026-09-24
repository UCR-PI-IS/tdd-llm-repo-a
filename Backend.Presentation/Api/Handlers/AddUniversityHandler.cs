using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for adding a new university.
/// </summary>
public static class AddUniversityHandler
{
    /// <summary>
    /// Handles the asynchronous request to add a new university.
    /// </summary>
    /// <param name="service">The university service.</param>
    /// <param name="dto">The data transfer object containing the university information.</param>
    /// <returns>
    /// A result wrapping the created university response,
    /// or a bad request if validation fails or the university already exists.
    /// </returns>
    public static async Task<Results<Created<AddUniversityResponse>, BadRequest<string>>> HandleAsync(
        IUniversityService service,
        AddUniversityDto dto)
    {
        var result = await service.AddUniversityAsync(dto.Name, dto.Country);

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest(result.ErrorMessage!);
        }

        return TypedResults.Created("/universities", new AddUniversityResponse { Message = "University added successfully" });
    }
}
