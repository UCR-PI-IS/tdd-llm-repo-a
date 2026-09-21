namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful person creation.
/// </summary>
/// <param name="Success">Indicates whether the creation was successful.</param>
/// <param name="PersonId">The unique identifier of the created person.</param>
public record class CreatePersonResponse(bool Success, string PersonId);
