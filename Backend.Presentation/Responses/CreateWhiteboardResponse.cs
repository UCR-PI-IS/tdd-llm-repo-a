using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the created whiteboard data.
/// </summary>
/// <param name="Whiteboard">The created whiteboard entity.</param>
public record class CreateWhiteboardResponse(Whiteboard Whiteboard);
