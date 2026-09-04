using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Builds a whiteboard entity from a request and validates it fits in the learning space.
/// </summary>
internal static class WhiteboardBuilder
{
    /// <summary>
    /// Creates a whiteboard from the request and validates it fits in the learning space.
    /// </summary>
    /// <exception cref="ValidationException">Thrown when the whiteboard doesn't fit in the learning space.</exception>
    public static Whiteboard Build(CreateWhiteboardRequest request, LearningSpace space)
    {
        var whiteboard = new Whiteboard(
            Guid.NewGuid().ToString(),
            request.LearningSpaceId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation,
            request.MarkerColor);

        if (!whiteboard.FitsInSpace(space))
        {
            throw new ValidationException("Whiteboard does not fit in learning space");
        }

        return whiteboard;
    }
}
