using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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
    /// <param name="service">The learning space create service.</param>
    /// <param name="dto">The data transfer object containing learning space details.</param>
    /// <returns>An HTTP result indicating success or failure.</returns>
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> 
        HandleAsync(ILearningSpaceCreateService service, CreateLearningSpaceDto dto)
    {
        try
        {
            var learningSpace = await service.CreateLearningSpaceAsync(
                dto.Id, 
                dto.Type, 
                dto.Height, 
                dto.Width, 
                dto.Length);

            var response = new LearningSpaceResponse(
                learningSpace.id,
                learningSpace.type,
                learningSpace.height,
                learningSpace.width,
                learningSpace.length);

            return TypedResults.Created($"/api/learningspaces/{response.Id}", response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return TypedResults.Problem(
                title: "Internal Server Error",
                detail: "An unexpected error occurred while creating the learning space.",
                statusCode: 500);
        }
    }
}
