namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    private static int _nextId = 1;

    /// <summary>
    /// Unique identifier for the learning space (auto-generated).
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
    /// Parameterless constructor for EF Core.
    /// </summary>
    private LearningSpace()
    {
        Type = string.Empty;
    }

    /// <summary>
    /// Constructor for the LearningSpace class with validation.
    /// </summary>
    /// <param name="type">Type of the learning space (Classroom, Auditorium, or Laboratory)</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(string type, float height, float width, float length)
    {
        // Validate type
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentException("Type is required", nameof(type));
        }

        var validTypes = new[] { "Classroom", "Auditorium", "Laboratory" };
        if (!validTypes.Contains(type))
        {
            throw new ArgumentException("Type must be Classroom, Auditorium, or Laboratory", nameof(type));
        }

        // Validate dimensions
        if (height <= 0)
        {
            throw new ArgumentException("Height must be positive and non-zero", nameof(height));
        }

        if (width <= 0)
        {
            throw new ArgumentException("Width must be positive and non-zero", nameof(width));
        }

        if (length <= 0)
        {
            throw new ArgumentException("Length must be positive and non-zero", nameof(length));
        }

        LearningSpaceId = _nextId++;
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    /// <summary>
    /// Resets the ID counter (for testing purposes).
    /// </summary>
    public static void ResetIdCounter()
    {
        _nextId = 1;
    }
}
