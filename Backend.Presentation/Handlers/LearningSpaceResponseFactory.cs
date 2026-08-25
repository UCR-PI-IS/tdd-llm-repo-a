using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Factory for creating HTTP responses related to learning space creation operations.
/// </summary>
internal static class LearningSpaceResponseFactory
{
    /// <summary>
    /// Executes the learning space creation workflow and returns the appropriate HTTP result.
    /// </summary>
    /// <param name="service">The learning space creation service.</param>
    /// <param name="type">Type of the learning space.</param>
    /// <param name="height">Height of the learning space in meters.</param>
    /// <param name="width">Width of the learning space in meters.</param>
    /// <param name="length">Length of the learning space in meters.</param>
    /// <returns>The appropriate HTTP result for the operation outcome.</returns>
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> ExecuteAsync(
        ILearningSpaceCreateService service, string type, float height, float width, float length)
    {
        try
        {
            var learningSpace = await service.CreateLearningSpaceAsync(type, height, width, length);
            var response = new LearningSpaceResponse(
                learningSpace.LearningSpaceId,
                learningSpace.Type,
                learningSpace.Height,
                learningSpace.Width,
                learningSpace.Length);
            return TypedResults.Created($"/LearningSpaces/{response.LearningSpaceId}", response);
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
