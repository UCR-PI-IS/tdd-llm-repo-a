using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Mappers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for fetching a list of learning components for a specific learning space.
/// </summary>
public static class GetLearningComponentsHandler
{
    /// <summary>
    /// Handles the asynchronous request to fetch learning components for a specific learning space.
    /// </summary>
    /// <param name="service">Service for accessing learning components.</param>
    /// <param name="learningSpaceId">The identifier of the learning space.</param>
    /// <returns>An result containing the list of learning components, or an error response.</returns>
    public static async Task<object> HandleAsync(
        ILearningComponentService service,
        string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
        {
            return CreateErrorResponse("Learning space ID cannot be null or empty");
        }

        try
        {
            var components = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);
            var componentDtos = components.Select(LearningComponentMapper.ToDto).ToList();
            var response = new GetLearningComponentsResponse(componentDtos);
            return TypedResults.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return CreateNotFoundResponse(ex.Message);
        }
    }

    private static BadRequest<ErrorResponse> CreateErrorResponse(string message)
    {
        var errorResponse = new ErrorResponse { Message = message };
        return TypedResults.BadRequest(errorResponse);
    }

    private static NotFound<ErrorResponse> CreateNotFoundResponse(string message)
    {
        var errorResponse = new ErrorResponse { Message = message };
        return TypedResults.NotFound(errorResponse);
    }
}
