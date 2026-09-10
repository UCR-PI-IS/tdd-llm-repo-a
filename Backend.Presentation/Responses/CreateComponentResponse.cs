namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response returned when a learning component is successfully created.
/// </summary>
public class CreateComponentResponse
{
    /// <summary>
    /// The component ID.
    /// </summary>
    public required string ComponentId { get; set; }

    /// <summary>
    /// A message describing the outcome.
    /// </summary>
    public required string Message { get; set; }
}
