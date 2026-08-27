namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Data transfer object for creating a new learning space.
/// </summary>
/// <param name="Type">The type of the learning space (Classroom, Auditorium, or Laboratory).</param>
/// <param name="Height">The height of the learning space in meters.</param>
/// <param name="Width">The width of the learning space in meters.</param>
/// <param name="Length">The length of the learning space in meters.</param>
public record class CreateLearningSpaceDto(
    string Type,
    float Height,
    float Width,
    float Length);
