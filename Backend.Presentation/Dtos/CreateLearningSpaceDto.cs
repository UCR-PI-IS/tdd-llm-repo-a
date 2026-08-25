namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for creating a learning space.
/// </summary>
/// <param name="Type">The type of the learning space (Classroom, Auditorium, or Laboratory).</param>
/// <param name="Height">Height of the learning space in meters.</param>
/// <param name="Width">Width of the learning space in meters.</param>
/// <param name="Length">Length of the learning space in meters.</param>
public record class CreateLearningSpaceDto(string Type, float Height, float Width, float Length);
