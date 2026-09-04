using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the created whiteboard.
/// </summary>
public record class CreateWhiteboardResponse(Whiteboard Whiteboard);
