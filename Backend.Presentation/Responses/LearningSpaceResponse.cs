namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a learning space operation.
/// </summary>
/// <param name="LearningSpaceId">The unique identifier of the learning space.</param>
/// <param name="Type">The type of the learning space.</param>
/// <param name="Height">The height of the learning space in meters.</param>
/// <param name="Width">The width of the learning space in meters.</param>
/// <param name="Length">The length of the learning space in meters.</param>
public record class LearningSpaceResponse(int LearningSpaceId, string Type, float Height, float Width, float Length);