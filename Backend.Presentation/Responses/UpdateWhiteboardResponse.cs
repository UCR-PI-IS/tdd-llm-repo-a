using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

/// <summary>
/// Response object carrying the updated whiteboard data.
/// </summary>
/// <param name="Whiteboard">The whiteboard DTO.</param>
public record class UpdateWhiteboardResponse(WhiteboardDto Whiteboard);
