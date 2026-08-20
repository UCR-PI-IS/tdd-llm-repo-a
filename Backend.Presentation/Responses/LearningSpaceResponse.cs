namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the details of a single learning space.
/// </summary>
/// <param name="LearningSpaceId">The auto-generated internal identifier.</param>
/// <param name="Type">The type of the learning space.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="Length">Length in meters.</param>
public record class LearningSpaceResponse(
    int LearningSpaceId,
    string Type,
    float Height,
    float Width,
    float Length);
