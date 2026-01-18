namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a component in a learning space (e.g., Whiteboard, Projector).
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// The name of the learning component.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructs a new learning component with the specified name.
    /// </summary>
    public LearningComponent(string name)
    {
        Name = name;
    }
}
