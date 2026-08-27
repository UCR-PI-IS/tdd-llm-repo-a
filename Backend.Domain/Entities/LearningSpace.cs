namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 1;

    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public int LearningSpaceId { get; private set; }

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
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="type">Type of the learning space (Classroom, Auditorium, or Laboratory)</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public LearningSpace(string type, float height, float width, float length)
    {
        ValidateType(type);
        ValidateHeight(height);
        ValidateWidth(width);
        ValidateLength(length);

        LearningSpaceId = _nextId++;
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    /// <summary>
    /// Validates that the type is not null/empty and is one of the allowed values.
    /// </summary>
    private static void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type is required", nameof(type));
        }

        var validTypes = new[] { "Classroom", "Auditorium", "Laboratory" };
        if (!validTypes.Contains(type))
        {
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));
        }
    }

    /// <summary>
    /// Validates that height is positive and non-zero.
    /// </summary>
    private static void ValidateHeight(float height)
    {
        if (height <= 0)
        {
            throw new ArgumentException("Height must be positive and non-zero", nameof(height));
        }
    }

    /// <summary>
    /// Validates that width is positive and non-zero.
    /// </summary>
    private static void ValidateWidth(float width)
    {
        if (width <= 0)
        {
            throw new ArgumentException("Width must be positive and non-zero", nameof(width));
        }
    }

    /// <summary>
    /// Validates that length is positive and non-zero.
    /// </summary>
    private static void ValidateLength(float length)
    {
        if (length <= 0)
        {
            throw new ArgumentException("Length must be positive and non-zero", nameof(length));
        }
    }
}
