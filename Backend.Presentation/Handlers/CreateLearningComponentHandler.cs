using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning component.
/// </summary>
public static class CreateLearningComponentHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning component.
    /// </summary>
    /// <param name="service">The learning component service.</param>
    /// <param name="request">The creation request.</param>
    /// <returns>An HTTP result.</returns>
    public static async Task<IResult> HandleAsync(
        ILearningComponentService service,
        CreateComponentRequest request)
    {
        try
        {
            var result = await service.CreateComponentAsync(request);
            var response = new CreateComponentResponse
            {
                ComponentId = result.ComponentId,
                Message = result.Message
            };
            return TypedResults.Created($"/api/components/{result.ComponentId}", response);
        }
        catch (DuplicateIdException ex)
        {
            return TypedResults.Conflict(new ErrorResponse { StatusCode = 409, Message = ex.Message });
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse { StatusCode = 400, Message = ex.Message });
        }
    }
}
