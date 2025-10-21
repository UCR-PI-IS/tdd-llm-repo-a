namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos.Spaces;

/// <summary>
/// Data Transfer Object (DTO) that represents a detailed view of a learning space.
/// </summary>
/// <param name="Name">
/// Name of the learning space.
/// </param>
/// <param name="Type">
/// Type of the learning space (e.g., classroom, lab, auditorium).
/// </param>
/// <param name="MaxCapacity">
/// Maximum capacity of the learning space (e.g., number of students).
/// </param>
/// <param name="Height">
/// Height of the learning space in meters.
/// </param>
/// <param name="Width">
/// Width of the learning space in meters.
/// </param>
/// <param name="Length">
/// Length of the learning space in meters.
/// </param>
public record class LearningSpaceDto(
    string Name,
    string Type,
    int MaxCapacity,
    double Height,
    double Width,
    double Length
);
