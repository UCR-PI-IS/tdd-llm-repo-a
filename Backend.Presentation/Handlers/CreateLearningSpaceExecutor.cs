using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Executes the create-learning-space use case, including error handling and response mapping.
/// </summary>
internal static class CreateLearningSpaceExecutor
{
    /// <summary>
    /// Creates a learning space via the service and maps the result to an HTTP response.
    /// </summary>
    /// <param name="service">The learning space create service.</param>
    /// <param name="dto">The data transfer object containing the creation data.</param>
    /// <returns>A typed HTTP result representing success or failure.</returns>
    public static async Task<Results<Created<LearningSpaceResponse>, BadRequest<string>, ProblemHttpResult>> ExecuteAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        try
        {
            var response = await LearningSpaceResponseMapper.CreateAndMapAsync(service, dto);
            return TypedResults.Created($"/LearningSpace/{response.LearningSpaceId}", response);
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
