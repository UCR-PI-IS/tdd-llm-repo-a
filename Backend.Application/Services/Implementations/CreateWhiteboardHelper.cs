using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Helper for executing whiteboard creation logic.
/// </summary>
internal static class CreateWhiteboardHelper
{
    /// <summary>
    /// Executes the create whiteboard workflow.
    /// </summary>
    public static async Task<Whiteboard> ExecuteAsync(
        CreateWhiteboardRequest request,
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceReadRepository learningSpaceReadRepository)
    {
        var learningSpace = await learningSpaceReadRepository.GetByIdAsync(request.LearningSpaceId);
        if (learningSpace == null)
            throw new NotFoundException("Learning space not found");

        var whiteboard = new Whiteboard(
            request.ComponentId,
            request.LearningSpaceId,
            request.Width,
            request.Height,
            request.Depth,
            request.X,
            request.Y,
            request.Z,
            request.Orientation,
            request.MarkerColor);

        if (!whiteboard.FitsInSpace(learningSpace))
            throw new ValidationException("Whiteboard does not fit in the learning space");

        await whiteboardRepository.AddAsync(whiteboard);
        return whiteboard;
    }
}
