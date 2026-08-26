using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning space.
/// </summary>
public static class CreateLearningSpaceHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning space.
    /// </summary>
    /// <param name="service">Service for creating learning spaces.</param>
    /// <param name="dto">Data transfer object containing the learning space details.</param>
    /// <returns>A result indicating the outcome of the operation.</returns>
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
            return TypedResults.Created($"/api/learningspaces/{response.LearningSpaceId}", response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem("An unexpected error occurred while creating the learning space.");
        }
    }
}
