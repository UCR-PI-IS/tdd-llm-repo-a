namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Result object returned after creating a learning component.
/// </summary>
public class CreateComponentResult
{
    public string ComponentId { get; }

    public CreateComponentResult(string componentId)
    {
        ComponentId = componentId;
    }
}
