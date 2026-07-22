using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

public class LearningComponentDto
{
    public string ComponentId { get; set; }
    public string LearningSpaceId { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Orientation { get; set; }
}

public class GetLearningComponentsResponse
{
    public List<LearningComponentDto> Components { get; set; } = new();
}
