namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

public class LearningComponent
{
    public int ComponentId { get; private set; }
    public string LearningSpaceId { get; private set; }
    public float Width { get; private set; }
    public float Height { get; private set; }
    public float Depth { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }
    public string Orientation { get; private set; }

    public LearningComponent(int componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        ComponentId = componentId;
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }
}
