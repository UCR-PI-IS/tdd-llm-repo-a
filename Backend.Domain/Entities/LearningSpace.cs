namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public String id { get; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public String type { get; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float height { get; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float width { get; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float length { get; }

    /// <summary>
    /// Constructor for the LearningSpace class with validation.
    /// </summary>
    /// <param name="id">Unique identifier for the learning space</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    /// <exception cref="ArgumentException">Thrown when validation fails</exception>
    public LearningSpace(String id, String type, float height, float width, float length)
    {
        ValidateParameters(id, type, height, width, length);

        this.id = id;
        this.type = type;
        this.height = height;
        this.width = width;
        this.length = length;
    }

    /// <summary>
    /// Validates all constructor parameters.
    /// </summary>
    private static void ValidateParameters(string id, string type, float height, float width, float length)
    {
        ValidateId(id);
        ValidateType(type);
        ValidateDimension(height, nameof(height));
        ValidateDimension(width, nameof(width));
        ValidateDimension(length, nameof(length));
    }

    /// <summary>
    /// Validates the learning space identifier.
    /// </summary>
    private static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id is required", nameof(id));
        }
    }

    /// <summary>
    /// Validates the learning space type.
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
    /// Validates a dimension value (height, width, or length).
    /// </summary>
    private static void ValidateDimension(float value, string paramName)
    {
        if (value <= 0)
        {
            throw new ArgumentException($"{char.ToUpper(paramName[0])}{paramName.Substring(1)} must be positive and non-zero", paramName);
        }
    }
}
