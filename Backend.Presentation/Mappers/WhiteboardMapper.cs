using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

/// <summary>
/// Maps between whiteboard DTOs, requests, and responses.
/// </summary>
internal static class WhiteboardMapper
{
    /// <summary>
    /// Converts a <see cref="CreateWhiteboardDto"/> to a <see cref="CreateWhiteboardRequest"/>.
    /// </summary>
    /// <param name="dto">The DTO to convert.</param>
    /// <returns>A new <see cref="CreateWhiteboardRequest"/> instance.</returns>
    public static CreateWhiteboardRequest ToRequest(CreateWhiteboardDto dto) =>
        new(dto.ComponentId, dto.LearningSpaceId,
            dto.Width, dto.Height, dto.Depth,
            dto.X, dto.Y, dto.Z,
            dto.Orientation, dto.MarkerColor);

    /// <summary>
    /// Converts a <see cref="Whiteboard"/> entity to a <see cref="CreateWhiteboardResponse"/>.
    /// </summary>
    /// <param name="whiteboard">The entity to convert.</param>
    /// <returns>A new <see cref="CreateWhiteboardResponse"/> instance.</returns>
    public static CreateWhiteboardResponse ToResponse(Whiteboard whiteboard) =>
        new(whiteboard);
}
