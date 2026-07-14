namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the component.</param>
/// <param name="LearningSpaceId">The identifier of the learning space where the component is located.</param>
/// <param name="Width">The width of the component in meters.</param>
/// <param name="Height">The height of the component in meters.</param>
/// <param name="Depth">The depth of the component in meters.</param>
/// <param name="X">The X coordinate position.</param>
/// <param name="Y">The Y coordinate position.</param>
/// <param name="Z">The Z coordinate position.</param>
/// <param name="Orientation">The orientation of the component.</param>
public record class LearningComponentDto(
    string ComponentId, 
    string LearningSpaceId, 
    float Width, 
    float Height, 
    float Depth, 
    float X, 
    float Y, 
    float Z, 
    string Orientation);
