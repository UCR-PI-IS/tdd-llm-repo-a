namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for learning component creation operations.
/// </summary>
/// <param name="ComponentId">The component identifier (set on success).</param>
/// <param name="Message">A descriptive message about the operation result.</param>
/// <param name="StatusCode">The HTTP status code of the response.</param>
/// <param name="ErrorMessage">An error message (set on failure).</param>
public record class CreateLearningComponentResponse(
    string? ComponentId,
    string? Message,
    int StatusCode,
    string? ErrorMessage);
