namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the created learning component data.
/// </summary>
/// <param name="ComponentId">The unique identifier of the created component.</param>
public record class CreateLearningComponentResponse(string ComponentId);
