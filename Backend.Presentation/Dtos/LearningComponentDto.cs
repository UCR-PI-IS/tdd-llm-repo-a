namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the component.</param>
/// <param name="LearningSpaceId">The identifier of the learning space.</param>
/// <param name="Width">The width of the component.</param>
/// <param name="Height">The height of the component.</param>
/// <param name="Depth">The depth of the component.</param>
/// <param name="X">The X coordinate of the component.</param>
/// <param name="Y">The Y coordinate of the component.</param>
/// <param name="Z">The Z coordinate of the component.</param>
/// <param name="Orientation">The orientation of the component.</param>
public record class LearningComponentDto(
    String ComponentId,
    String LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    String Orientation);
