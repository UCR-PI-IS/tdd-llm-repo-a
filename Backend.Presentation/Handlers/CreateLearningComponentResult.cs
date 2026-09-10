namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Result object returned by the <see cref="CreateLearningComponentHandler"/>.
/// </summary>
public class CreateLearningComponentResult
{
    /// <summary>
    /// The identifier assigned to the created component, if successful.
    /// </summary>
    public string? ComponentId { get; set; }

    /// <summary>
    /// A human-readable message describing the result, if successful.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The HTTP status code for the result.
    /// </summary>
    public int StatusCode { get; set; } = 200;

    /// <summary>
    /// An error message, if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
