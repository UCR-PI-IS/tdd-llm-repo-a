namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result of creating a person.
/// </summary>
public class CreatePersonResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the creation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the created person.
    /// </summary>
    public Guid PersonId { get; set; }

    /// <summary>
    /// Gets or sets the error message if the creation failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
