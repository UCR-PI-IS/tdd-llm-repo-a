namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public String id { get; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public String type { get; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float height { get; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float width { get; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float length { get; }

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="id">Unique identifier for the learning space</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(String id, String type, float height, float width, float length)
    {
        this.id = id;
        this.type = type;
        this.height = height;
        this.width = width;
        this.length = length;
    }
}
