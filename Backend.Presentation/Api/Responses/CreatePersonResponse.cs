namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful person creation.
/// </summary>
public record class CreatePersonResponse(
    bool Success,
    int Id,
    string FirstName,
    string LastName,
    string Email);
