namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the result of a learning component creation request.
/// </summary>
/// <param name="ComponentId">The ID of the created component.</param>
/// <param name="Message">A success message.</param>
/// <param name="StatusCode">The HTTP status code.</param>
/// <param name="ErrorMessage">An error message if the request failed.</param>
public record class CreateLearningComponentResponse(
    string? ComponentId = null,
    string? Message = null,
    int StatusCode = 200,
    string? ErrorMessage = null);
