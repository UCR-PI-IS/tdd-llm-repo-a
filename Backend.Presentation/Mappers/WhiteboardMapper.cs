using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class WhiteboardMapper
{
    public static CreateWhiteboardRequest ToRequest(CreateWhiteboardDto dto)
    {
        return new CreateWhiteboardRequest(
            dto.ComponentId,
            dto.LearningSpaceId,
            dto.Width,
            dto.Height,
            dto.Depth,
            dto.X,
            dto.Y,
            dto.Z,
            dto.Orientation,
            dto.MarkerColor);
    }

    public static CreateWhiteboardResponse ToResponse(Whiteboard whiteboard)
    {
        var whiteboardDto = new WhiteboardDto(
            whiteboard.ComponentId,
            whiteboard.LearningSpaceId,
            whiteboard.Width,
            whiteboard.Height,
            whiteboard.Depth,
            whiteboard.X,
            whiteboard.Y,
            whiteboard.Z,
            whiteboard.Orientation,
            whiteboard.MarkerColor);

        return new CreateWhiteboardResponse(whiteboardDto);
    }

    public static UpdateWhiteboardDto ToUpdateDto(UpdateWhiteboardRequest request)
    {
        return new UpdateWhiteboardDto(
            request.WhiteboardId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation,
            request.MarkerColor);
    }

    public static UpdateWhiteboardResponse ToUpdateResponse(Whiteboard whiteboard)
    {
        return new UpdateWhiteboardResponse(
            whiteboard.ComponentId,
            whiteboard.LearningSpaceId,
            whiteboard.Width,
            whiteboard.Height,
            whiteboard.Depth,
            whiteboard.X,
            whiteboard.Y,
            whiteboard.Z,
            whiteboard.Orientation,
            whiteboard.MarkerColor);
    }
}
