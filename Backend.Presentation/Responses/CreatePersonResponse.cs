namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the result of a person creation operation.
/// </summary>
/// <param name="Success">Indicates whether the person was created successfully.</param>
public record class CreatePersonResponse(bool Success);
