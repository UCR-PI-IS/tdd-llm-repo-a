using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new learning space.
/// </summary>
public static class CreateLearningSpaceHandler
{
    /// <summary>
    /// Handles the asynchronous request to create a new learning space.
    /// </summary>
    /// <param name="service">The learning space creation service.</param>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response for the creation request.
    /// </returns>
    public static async Task<IResult> HandleAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        try
        {
            var learningSpace = await service.CreateLearningSpaceAsync(
                dto.Type, dto.Height, dto.Width, dto.Length);
            return LearningSpaceCreateResponseFactory.CreateCreatedResponse(learningSpace);
        }
        catch (ArgumentException ex)
        {
            return LearningSpaceCreateResponseFactory.CreateBadRequestResponse(ex.Message);
        }
        catch (Exception)
        {
            return LearningSpaceCreateResponseFactory.CreateErrorResponse();
        }
    }
}
