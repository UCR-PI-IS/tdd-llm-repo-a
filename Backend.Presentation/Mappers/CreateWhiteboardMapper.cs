using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

internal static class CreateWhiteboardMapper
{
    public static CreateWhiteboardRequest ToRequest(CreateWhiteboardDto dto) =>
        new(
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
