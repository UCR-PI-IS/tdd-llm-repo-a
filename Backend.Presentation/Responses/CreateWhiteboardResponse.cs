using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying whiteboard data after creation.
/// </summary>
/// <param name="Whiteboard">The created whiteboard entity.</param>
public record class CreateWhiteboardResponse(Whiteboard Whiteboard);
