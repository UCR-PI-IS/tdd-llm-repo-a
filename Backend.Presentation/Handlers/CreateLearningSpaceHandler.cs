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
    /// Handles the asynchronous request to create a learning space.
    /// </summary>
    /// <param name="service">The learning space create service.</param>
    /// <param name="dto">The data transfer object containing learning space details.</param>
    /// <returns>A <see cref="CreateLearningSpaceResult"/> representing the result of the operation.</returns>
    public static async Task<CreateLearningSpaceResult> HandleAsync(
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

            return CreateLearningSpaceResult.Success(learningSpace.LearningSpaceId, response);
        }
        catch (ArgumentException ex)
        {
            return CreateLearningSpaceResult.BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return CreateLearningSpaceResult.Problem("An unexpected error occurred while creating the learning space.");
        }
    }
}
