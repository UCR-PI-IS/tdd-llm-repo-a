namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for the create learning component operation.
/// </summary>
public class CreateLearningComponentResponse
{
    /// <summary>
    /// The unique identifier of the created component.
    /// </summary>
    public string? ComponentId { get; set; }

    /// <summary>
    /// A success message describing the result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The HTTP status code of the response.
    /// </summary>
    public int StatusCode { get; set; } = 201;

    /// <summary>
    /// An error message if the operation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
