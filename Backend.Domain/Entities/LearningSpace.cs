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
    /// Unique internal identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; set; }

    /// <summary>
    /// Type of the learning space (Classroom, Auditorium, or Laboratory).
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
    /// Generates a unique internal ID and validates all parameters.
    /// </summary>
    /// <param name="type">Type of the learning space (Classroom, Auditorium, or Laboratory).</param>
    /// <param name="height">Height of the learning space in meters. Must be positive.</param>
    /// <param name="width">Width of the learning space in meters. Must be positive.</param>
    /// <param name="length">Length of the learning space in meters. Must be positive.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        ValidateType(type);
        ValidateDimension(height, "Height");
        ValidateDimension(width, "Width");
        ValidateDimension(length, "Length");

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

    private static void ValidateDimension(float value, string name)
    {
        if (value <= 0f)
            throw new ArgumentException(
                $"{name} must be positive and non-zero",
                name.ToLowerInvariant());
    }
}
