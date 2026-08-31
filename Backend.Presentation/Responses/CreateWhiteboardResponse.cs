using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying created whiteboard data.
/// </summary>
/// <param name="Whiteboard">The created whiteboard entity.</param>
public record CreateWhiteboardResponse(Whiteboard Whiteboard);
