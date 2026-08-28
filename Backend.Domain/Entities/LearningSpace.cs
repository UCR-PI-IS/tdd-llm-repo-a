namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 1;

    private static readonly HashSet<string> ValidTypes = new()
    {
        "Classroom", "Auditorium", "Laboratory"
    };

    /// <summary>
    /// Unique internal identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; set; }

    /// <summary>
    /// Type of the learning space (e.g., Classroom, Auditorium, Laboratory).
    /// </summary>
    public string Type { get; private set; }

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
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private LearningSpace()
    {
        Type = string.Empty;
    }

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        ValidateType(type);
        ValidatePositiveDimension(height, nameof(height), "Height");
        ValidatePositiveDimension(width, nameof(width), "Width");
        ValidatePositiveDimension(length, nameof(length), "Length");

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

    private static void ValidatePositiveDimension(float value, string paramName, string displayName)
    {
        if (value <= 0f)
            throw new ArgumentException($"{displayName} must be positive and non-zero", paramName);
    }
}
