namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

/// <summary>
/// Request DTO for creating a learning component via API.
/// </summary>
public class CreateComponentRequest
{
    public string LearningSpaceId { get; set; } = string.Empty;
    public float Width { get; set; }
    public float Height { get; set; }
    public float Depth { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Orientation { get; set; } = string.Empty;
    public string? ComponentId { get; set; }
}
