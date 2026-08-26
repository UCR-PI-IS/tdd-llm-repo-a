namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 100;

    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float Length { get; }

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(string type, float height, float width, float length)
    {
        ValidateType(type);
        ValidatePositive(height, nameof(height), "Height");
        ValidatePositive(width, nameof(width), "Width");
        ValidatePositive(length, nameof(length), "Length");

        LearningSpaceId = _nextId++;
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    private static void ValidateType(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("Type is required", nameof(type));
        }

        var validTypes = new[] { "Classroom", "Auditorium", "Laboratory" };
        if (!validTypes.Contains(type))
        {
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));
        }
    }

    private static void ValidatePositive(float value, string paramName, string displayName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"{displayName} must be positive and non-zero", paramName);
        }
    }
}
