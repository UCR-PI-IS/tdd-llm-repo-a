namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

/// <summary>
/// Request for creating a whiteboard.
/// </summary>
public record CreateWhiteboardRequest(
    string WhiteboardId,
    double Width,
    double Height,
    string LearningSpaceId
);
