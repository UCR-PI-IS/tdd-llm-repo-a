namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

/// <summary>
/// Data transfer object for LearningComponent
/// </summary>
public class LearningComponentDto
{
    public int ComponentId { get; set; }
    public string LearningSpaceId { get; set; } = string.Empty;
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float ScaleZ { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float PositionZ { get; set; }
    public string Rotation { get; set; } = string.Empty;
}
