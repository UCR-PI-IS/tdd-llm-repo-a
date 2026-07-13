namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public String id { get; private set; } = null!;

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public String type { get; private set; } = null!;

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float height { get; private set; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float width { get; private set; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float length { get; private set; }

    /// <summary>
    /// Private parameterless constructor for Entity Framework.
    /// </summary>
    private LearningSpace()
    {
    }

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
