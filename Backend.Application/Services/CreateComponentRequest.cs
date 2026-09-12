using System.Text.Json.Serialization;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object for creating a new learning component.
/// </summary>
public class CreateComponentRequest
{
    /// <summary>
    /// Optional explicit identifier for the component.
    /// </summary>
    public string? ComponentId { get; init; }

    /// <summary>
    /// Identifier of the learning space this component belongs to.
    /// </summary>
    public string LearningSpaceId { get; }

    /// <summary>
    /// Width of the component in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Height of the component in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Depth of the component in meters.
    /// </summary>
    public float Depth { get; }

    /// <summary>
    /// X coordinate position.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// Y coordinate position.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Z coordinate position.
    /// </summary>
    public float Z { get; }

    /// <summary>
    /// Orientation of the component.
    /// </summary>
    public string Orientation { get; }

    /// <summary>
    /// Initializes a new instance without an explicit component ID.
    /// </summary>
    [JsonConstructor]
    public CreateComponentRequest(
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation)
    {
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

    /// <summary>
    /// Initializes a new instance with an explicit component ID.
    /// </summary>
    public CreateComponentRequest(
        string componentId,
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation)
        : this(learningSpaceId, width, height, depth, x, y, z, orientation)
    {
        ComponentId = componentId;
    }
}
