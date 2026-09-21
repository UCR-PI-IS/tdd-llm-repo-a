namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful person creation.
/// </summary>
public class CreatePersonResponse
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the unique identifier of the created person.
    /// </summary>
    public int PersonId { get; }

    /// <summary>
    /// Gets the message describing the result.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonResponse"/> class.
    /// </summary>
    /// <param name="success">Whether the operation was successful.</param>
    /// <param name="personId">The unique identifier of the created person.</param>
    /// <param name="message">The message describing the result.</param>
    public CreatePersonResponse(bool success, int personId, string message)
    {
        Success = success;
        PersonId = personId;
        Message = message;
    }
}
