namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 1;
    private static readonly string[] AllowedTypes = { "Classroom", "Auditorium", "Laboratory" };

    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; private set; }

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
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(width, nameof(width));
        ValidatePositiveDimension(length, nameof(length));

        Type = type;
        Height = height;
        Width = width;
        Length = length;
        LearningSpaceId = _nextId++;
    }

    private static void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type is required", nameof(type));
        }

        if (!AllowedTypes.Contains(type))
        {
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));
        }
    }

    private static void ValidatePositiveDimension(float value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"{char.ToUpper(paramName[0])}{paramName.Substring(1)} must be positive and non-zero", paramName);
        }
    }
}
