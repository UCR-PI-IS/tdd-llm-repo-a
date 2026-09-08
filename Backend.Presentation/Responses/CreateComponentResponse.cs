namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for the create learning component operation.
/// </summary>
/// <param name="ComponentId">The component identifier (generated or provided).</param>
/// <param name="Message">A descriptive message about the operation result.</param>
/// <param name="StatusCode">The HTTP status code.</param>
/// <param name="ErrorMessage">An error message if the operation failed; otherwise null.</param>
public record class CreateComponentResponse(
    string? ComponentId,
    string? Message,
    int StatusCode,
    string? ErrorMessage);
