namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for a successfully created person.
/// </summary>
/// <param name="Success">Whether the creation was successful.</param>
public record class CreatePersonResponse(bool Success);
