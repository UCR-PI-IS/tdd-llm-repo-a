namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful university creation.
/// </summary>
public class AddUniversityResponse
{
    /// <summary>
    /// Gets the success message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddUniversityResponse"/> class.
    /// </summary>
    /// <param name="message">The success message.</param>
    public AddUniversityResponse(string message)
    {
        Message = message;
    }
}
