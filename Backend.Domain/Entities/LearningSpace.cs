namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 0;

    private static readonly HashSet<string> ValidTypes = new()
    {
        "Classroom", "Auditorium", "Laboratory"
    };

    /// <summary>
    /// Unique internal identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; private set; }

    /// <summary>
    /// Type of the learning space (e.g., Classroom, Auditorium, Laboratory).
    /// </summary>
    public string Type { get; private set; } = string.Empty;

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float Height { get; private set; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float Width { get; private set; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float Length { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private LearningSpace() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningSpace"/> class with validation.
    /// </summary>
    /// <param name="type">Type of the learning space (Classroom, Auditorium, or Laboratory).</param>
    /// <param name="height">Height of the learning space in meters.</param>
    /// <param name="width">Width of the learning space in meters.</param>
    /// <param name="length">Length of the learning space in meters.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("Type is required", nameof(type));

        if (!ValidTypes.Contains(type))
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));

        ThrowIfNotPositive(height, "Height");
        ThrowIfNotPositive(width, "Width");
        ThrowIfNotPositive(length, "Length");

        LearningSpaceId = Interlocked.Increment(ref _nextId);
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    private static void ThrowIfNotPositive(float value, string displayName)
    {
        if (value <= 0f)
            throw new ArgumentException($"{displayName} must be positive and non-zero", displayName);
    }
}
