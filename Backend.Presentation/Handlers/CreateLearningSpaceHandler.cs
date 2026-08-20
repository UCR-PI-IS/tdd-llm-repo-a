using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning space.
/// </summary>
public static class CreateLearningSpaceHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a learning space.
    /// </summary>
    /// <param name="service">The learning space creation service.</param>
    /// <param name="dto">The creation request data.</param>
    /// <returns>A result indicating success or failure.</returns>
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> HandleAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        try
        {
            var response = LearningSpaceResponseMapper.ToResponse(
                await service.CreateLearningSpaceAsync(dto.Type, dto.Height, dto.Width, dto.Length));
            return TypedResults.Created($"/LearningSpace/{response.LearningSpaceId}", response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest<string>(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred.", statusCode: 500);
        }
    }
}
