using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.Spaces;
using UCR.ECCI.PI.ThemePark.Backend.Domain.ValueObjects.UniversityManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities.Spaces;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{

    /// <summary
    /// Unique identifier for the learning space (database-internal).
    /// </summary>
    public int LearningSpaceId { get; }

    /// <summary>
    /// Unique name of the learning space within the floor.
    /// </summary>
    public EntityName Name { get; set; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab, auditorium).
    /// </summary>
    public LearningSpaceType Type { get; set; }

    /// <summary>
    /// Maximum capacity of the people that can fit in the learning space.
    /// </summary>
    public Capacity MaxCapacity { get; set; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public Size Height { get; set; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public Size Width { get; set; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public Size Length { get; set; }

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="name">Name of the learning space (unique within the floor)</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="maxCapacity">Maximum capacity of the people that can fit in the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(EntityName name, LearningSpaceType type, Capacity maxCapacity, Size height, Size width, Size length)
    {
        Name = name;
        Type = type;
        MaxCapacity = maxCapacity;
        Height = height;
        Width = width;
        Length = length;
    }

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="id">Unique identifier for the learning space</param>
    /// <param name="name">Name of the learning space (unique within the floor)</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="maxCapacity">Maximum capacity of the people that can fit in the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(int id, EntityName name, LearningSpaceType type, Capacity maxCapacity, Size height, Size width, Size length)
    {
        LearningSpaceId = id;
        Name = name;
        Type = type;
        MaxCapacity = maxCapacity;
        Height = height;
        Width = width;
        Length = length;
    }
}
