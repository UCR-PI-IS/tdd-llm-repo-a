using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Helper for executing whiteboard update logic.
/// </summary>
internal static class UpdateWhiteboardHelper
{
    /// <summary>
    /// Executes the update whiteboard workflow.
    /// </summary>
    public static async Task<UpdateWhiteboardResult> ExecuteAsync(
        UpdateWhiteboardDto dto,
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository? learningSpaceRepository)
    {
        var existingWhiteboard = await whiteboardRepository.GetByIdAsync(dto.WhiteboardId);
        if (existingWhiteboard == null)
            return UpdateWhiteboardResult.Failure("Whiteboard not found");

        try
        {
            if (learningSpaceRepository != null)
            {
                var error = await ValidatePositionAsync(dto, existingWhiteboard, whiteboardRepository, learningSpaceRepository);
                if (error != null)
                    return UpdateWhiteboardResult.Failure(error);
            }

            existingWhiteboard.Update(
                dto.Width, dto.Height, dto.Depth,
                dto.X, dto.Y, dto.Z,
                dto.Orientation, dto.MarkerColor);

            await whiteboardRepository.UpdateAsync(existingWhiteboard);
            return UpdateWhiteboardResult.Success(existingWhiteboard);
        }
        catch (ArgumentException ex)
        {
            return UpdateWhiteboardResult.Failure($"Invalid marker color: {ex.Message}");
        }
    }

    private static async Task<string?> ValidatePositionAsync(
        UpdateWhiteboardDto dto,
        Whiteboard existingWhiteboard,
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        var learningSpace = await learningSpaceRepository.GetByIdAsync(existingWhiteboard.LearningSpaceId);
        if (learningSpace == null)
            return "Learning space not found";

        var existingComponents = await whiteboardRepository.GetByLearningSpaceIdAsync(existingWhiteboard.LearningSpaceId);

        try
        {
            PositionValidator.ValidateBoundary(
                dto.X, dto.Z, dto.Width, dto.Depth,
                learningSpace.Width, learningSpace.Length);
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }

        try
        {
            PositionValidator.ValidateNoOverlap(
                existingWhiteboard.ComponentId,
                dto.X, dto.Z, dto.Width, dto.Depth,
                existingComponents);
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }

        return null;
    }
}
