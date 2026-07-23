using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;

public class GetLearningComponentsResponse
{
    public List<LearningComponentDto> Components { get; set; } = new();
}

public class LearningComponentDto
{
    public Guid ComponentId { get; set; }
    public Guid LearningSpaceId { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Orientation { get; set; } = string.Empty;
}
