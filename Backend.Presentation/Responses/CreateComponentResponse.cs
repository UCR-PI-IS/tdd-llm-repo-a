namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object returned after creating a learning component.
/// </summary>
/// <param name="ComponentId">The unique identifier of the created component.</param>
/// <param name="Message">A message describing the outcome of the creation.</param>
public record class CreateComponentResponse(string ComponentId, string Message);
