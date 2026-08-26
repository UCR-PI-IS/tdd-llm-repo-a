namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a learning space.
/// </summary>
/// <param name="Type">The type of the learning space.</param>
/// <param name="Height">The height of the learning space.</param>
/// <param name="Width">The width of the learning space.</param>
/// <param name="Length">The length of the learning space.</param>
public record class CreateLearningSpaceDto(string Type, float Height, float Width, float Length);
