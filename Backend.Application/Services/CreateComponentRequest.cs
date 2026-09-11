namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Request object for creating a learning component.
/// </summary>
public class CreateComponentRequest
{
    public string LearningSpaceId { get; }
    public float Width { get; }
    public float Height { get; }
    public float Depth { get; }
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    public string Orientation { get; }
    public string? ComponentId { get; }

    public CreateComponentRequest(
        string learningSpaceId,
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string orientation,
        string? componentId)
    {
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
        ComponentId = componentId;
    }
}
