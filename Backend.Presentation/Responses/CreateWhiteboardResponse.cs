using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the created whiteboard data.
/// </summary>
/// <param name="Whiteboard">The whiteboard DTO.</param>
public record class CreateWhiteboardResponse(WhiteboardDto Whiteboard);
