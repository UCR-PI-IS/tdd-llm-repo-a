namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new learning space.
/// </summary>
/// <param name="Type">Type of the learning space.</param>
/// <param name="Height">Height in meters.</param>
/// <param name="Width">Width in meters.</param>
/// <param name="Length">Length in meters.</param>
public record class CreateLearningSpaceDto(string Type, float Height, float Width, float Length);
