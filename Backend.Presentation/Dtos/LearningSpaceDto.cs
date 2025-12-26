namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Represents the data transfer object for a learning space.
/// </summary>
/// <param name="Id">The unique identifier of the learning space.</param>
/// <param name="Type">The type or category of the learning space.</param>
public record class LearningSpaceDto(string Id, string Type);
