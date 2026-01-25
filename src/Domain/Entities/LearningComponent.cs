namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities
{
    /// <summary>
    /// Represents a learning component in a learning space.
    /// </summary>
    public class LearningComponent
    {
        public int ComponentId { get; }
        public string LearningSpaceId { get; }
        public float Width { get; }
        public float Height { get; }
        public float Depth { get; }
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        public string Orientation { get; }

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
}
