using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.AspNetCore.Http.TypedResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating learning spaces.
/// </summary>
public static class CreateLearningSpaceHandler
{
    /// <summary>
    /// Handles the creation of a learning space.
    /// </summary>
    /// <param name="service">The learning space creation service.</param>
    /// <param name="dto">The data transfer object containing the learning space details.</param>
    /// <returns>The result of the creation operation.</returns>
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> HandleAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        try
        {
            var learningSpace = await service.CreateLearningSpaceAsync(dto.Type, dto.Height, dto.Width, dto.Length);
            var response = new LearningSpaceResponse(
                learningSpace.LearningSpaceId,
                learningSpace.Type,
                learningSpace.Height,
                learningSpace.Width,
                learningSpace.Length);
            return Created($"/api/learningspaces/{learningSpace.LearningSpaceId}", response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return Problem("An unexpected error occurred while creating the learning space.");
        }
    }
}
