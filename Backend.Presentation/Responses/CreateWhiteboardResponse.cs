namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying whiteboard data.
/// </summary>
/// <param name="ComponentId">The unique identifier of the whiteboard.</param>
/// <param name="LearningSpaceId">The identifier of the learning space where the whiteboard is placed.</param>
/// <param name="Width">Width of the whiteboard in meters.</param>
/// <param name="Height">Height of the whiteboard in meters.</param>
/// <param name="Depth">Depth of the whiteboard in meters.</param>
/// <param name="X">X coordinate position within the learning space.</param>
/// <param name="Y">Y coordinate position within the learning space.</param>
/// <param name="Z">Z coordinate position within the learning space.</param>
/// <param name="Orientation">Orientation of the whiteboard.</param>
/// <param name="MarkerColor">Color of the markers for the whiteboard.</param>
public record class WhiteboardDto(
    string ComponentId,
    string LearningSpaceId,
    float Width,
    float Height,
    float Depth,
    float X,
    float Y,
    float Z,
    string Orientation,
    string MarkerColor);

/// <summary>
/// Response object for creating a whiteboard.
/// </summary>
/// <param name="Whiteboard">The created whiteboard data.</param>
public record class CreateWhiteboardResponse(WhiteboardDto Whiteboard);
