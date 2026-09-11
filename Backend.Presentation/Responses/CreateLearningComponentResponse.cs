namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response DTO for creating a learning component.
/// </summary>
public class CreateLearningComponentResponse
{
    public string ComponentId { get; }
    public string Message { get; }

    public CreateLearningComponentResponse(string componentId, string message)
    {
        ComponentId = componentId;
        Message = message;
    }
}
