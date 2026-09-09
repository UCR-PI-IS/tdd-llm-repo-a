namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object returned after successfully creating a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the created component.</param>
/// <param name="LearningSpaceId">The identifier of the learning space.</param>
/// <param name="Width">Width of the component in meters.</param>
/// <param name="Height">Height of the component in meters.</param>
/// <param name="Depth">Depth of the component in meters.</param>
/// <param name="X">X coordinate position.</param>
/// <param name="Y">Y coordinate position.</param>
/// <param name="Z">Z coordinate position.</param>
/// <param name="Orientation">Orientation of the component.</param>
public record class CreateLearningComponentResponse(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation);
