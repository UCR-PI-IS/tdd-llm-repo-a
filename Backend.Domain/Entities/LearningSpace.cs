namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId;

    private static readonly HashSet<string> ValidTypes = new()
    {
        "Classroom", "Auditorium", "Laboratory"
    };

    /// <summary>
    /// Auto-generated internal identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; }

    /// <summary>
    /// Type of the learning space (e.g., Classroom, Auditorium, Laboratory).
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
    /// Initializes a new instance of the <see cref="LearningSpace"/> class
    /// with the specified type and dimensions.
    /// </summary>
    /// <param name="type">Type of the learning space.</param>
    /// <param name="height">Height of the learning space in meters.</param>
    /// <param name="width">Width of the learning space in meters.</param>
    /// <param name="length">Length of the learning space in meters.</param>
    /// <exception cref="ArgumentException">Thrown when type is null/empty, invalid, or dimensions are non-positive.</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        ValidateType(type);
        ValidatePositiveDimension(height, nameof(height));
        ValidatePositiveDimension(width, nameof(width));
        ValidatePositiveDimension(length, nameof(length));

        LearningSpaceId = Interlocked.Increment(ref _nextId);
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    private static void ValidateType(string type)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("Type is required", nameof(type));

        if (!ValidTypes.Contains(type))
            throw new ArgumentException(
                "Type must be Classroom, Auditorium, or Laboratory",
                nameof(type));
    }

    private static void ValidatePositiveDimension(float value, string paramName)
    {
        if (value <= 0f)
        {
            var displayName = char.ToUpper(paramName[0]) + paramName[1..];
            throw new ArgumentException($"{displayName} must be positive and non-zero", paramName);
        }
    }
}
