namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the data of a created learning space.
/// </summary>
/// <param name="LearningSpaceId">The unique internal identifier of the learning space.</param>
/// <param name="Type">The type of the learning space.</param>
/// <param name="Height">Height of the learning space in meters.</param>
/// <param name="Width">Width of the learning space in meters.</param>
/// <param name="Length">Length of the learning space in meters.</param>
public record class LearningSpaceResponse(int LearningSpaceId, string Type, float Height, float Width, float Length);
