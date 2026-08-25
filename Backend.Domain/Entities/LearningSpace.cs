namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static readonly HashSet<string> ValidTypes = new()
    {
        "Classroom", "Auditorium", "Laboratory"
    };

    /// <summary>
    /// Unique internal identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; private set; }

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
    /// </summary>
    /// <param name="type">Type of the learning space (Classroom, Auditorium, or Laboratory)</param>
    /// <param name="height">Height of the learning space in meters. Must be positive and non-zero.</param>
    /// <param name="width">Width of the learning space in meters. Must be positive and non-zero.</param>
    /// <param name="length">Length of the learning space in meters. Must be positive and non-zero.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("Type is required", nameof(type));

        if (!ValidTypes.Contains(type))
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));

        EnsurePositive(height, nameof(height), "Height");
        EnsurePositive(width, nameof(width), "Width");
        EnsurePositive(length, nameof(length), "Length");

        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    private static void EnsurePositive(float value, string paramName, string displayName)
    {
        if (value <= 0.0f)
            throw new ArgumentException($"{displayName} must be positive and non-zero", paramName);
    }
}
