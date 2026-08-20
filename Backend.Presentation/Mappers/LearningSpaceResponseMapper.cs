using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps creation requests to <see cref="LearningSpaceResponse"/> instances
/// by invoking the create service and converting the resulting domain entity.
/// </summary>
internal static class LearningSpaceResponseMapper
{
    /// <summary>
    /// Invokes the create service with the DTO values and maps the resulting entity to a response.
    /// </summary>
    /// <param name="service">The learning space create service.</param>
    /// <param name="dto">The data transfer object containing the creation data.</param>
    /// <returns>A <see cref="LearningSpaceResponse"/> representing the created learning space.</returns>
    public static async Task<LearningSpaceResponse> CreateAndMapAsync(
        ILearningSpaceCreateService service,
        CreateLearningSpaceDto dto)
    {
        var learningSpace = await service.CreateLearningSpaceAsync(dto.Type, dto.Height, dto.Width, dto.Length);
        return new LearningSpaceResponse(
            learningSpace.LearningSpaceId,
            learningSpace.Type,
            learningSpace.Height,
            learningSpace.Width,
            learningSpace.Length);
    }
}
