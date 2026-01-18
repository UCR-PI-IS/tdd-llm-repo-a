namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning component in a learning space.
/// </summary>
public class LearningComponent
{
    /// <summary>
    /// Gets the name of the learning component.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponent"/> class.
    /// </summary>
    /// <param name="name">The name of the learning component.</param>
    public LearningComponent(string name)
    {
        Name = name;
    }
}