namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a component (e.g., Whiteboard, Projector) in a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Name of the component.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructor for the LearningComponent class.
    /// </summary>
    /// <param name="name">Name of the component.</param>
    public LearningComponent(string name)
    {
        Name = name;
    }
}
