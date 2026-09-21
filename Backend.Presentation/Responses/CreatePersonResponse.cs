namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object for successful person creation.
/// </summary>
public class CreatePersonResponse
{
    /// <summary>
    /// Gets or sets a value indicating whether the creation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the created person.
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the message associated with the response.
    /// </summary>
    public string? Message { get; set; }
}
