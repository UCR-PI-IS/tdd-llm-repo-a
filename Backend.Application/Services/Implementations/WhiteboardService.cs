using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for creating and updating whiteboards.
/// </summary>
internal class WhiteboardService : IWhiteboardCreateService, IWhiteboardService
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceRepository">The learning space repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    private async Task<LearningSpace?> GetLearningSpaceAsync(string id)
    {
        return await _learningSpaceRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Creates a new whiteboard, validates it fits in the learning space, and persists it.
    /// </summary>
    /// <param name="request">The creation request containing whiteboard parameters.</param>
    /// <returns>The created whiteboard entity.</returns>
    /// <exception cref="NotFoundException">Thrown when the learning space does not exist.</exception>
    /// <exception cref="ValidationException">Thrown when the whiteboard does not fit in the learning space.</exception>
    public async Task<Whiteboard> CreateWhiteboardAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await GetLearningSpaceAsync(request.LearningSpaceId);
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

        await _whiteboardRepository.AddAsync(whiteboard);
        return whiteboard;
    }

    /// <summary>
    /// Updates an existing whiteboard with new data.
    /// </summary>
    /// <param name="dto">The update data transfer object.</param>
    /// <returns>A result indicating success or failure with an error message.</returns>
    public async Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto)
    {
        // Get the existing whiteboard
        var existingWhiteboard = await _whiteboardRepository.GetByIdAsync(dto.WhiteboardId);
        if (existingWhiteboard == null)
        {
            return UpdateWhiteboardResult.Failure("Whiteboard not found");
        }

        // Validate marker color
        if (string.IsNullOrEmpty(dto.MarkerColor))
        {
            return UpdateWhiteboardResult.Failure("Invalid marker color");
        }

        // Validate position boundaries
        var boundaryCheckResult = await ValidatePositionBoundariesAsync(dto, existingWhiteboard);
        if (boundaryCheckResult != null)
        {
            return boundaryCheckResult;
        }

        // Validate no overlap with other components
        var overlapCheckResult = await ValidateNoOverlapAsync(dto, existingWhiteboard);
        if (overlapCheckResult != null)
        {
            return overlapCheckResult;
        }

        return await PerformUpdateAsync(dto, existingWhiteboard);
    }

    private async Task<UpdateWhiteboardResult?> ValidatePositionBoundariesAsync(UpdateWhiteboardDto dto, Whiteboard existingWhiteboard)
    {
        var learningSpace = await GetLearningSpaceAsync(existingWhiteboard.LearningSpaceId);
        if (learningSpace != null)
        {
            // Check if position exceeds learning space boundaries
            if (dto.X + dto.Width > learningSpace.Width || dto.Z + dto.Depth > learningSpace.Length)
            {
                return UpdateWhiteboardResult.Failure("Position exceeds learning space boundaries");
            }
        }
        return null;
    }

    private async Task<UpdateWhiteboardResult?> ValidateNoOverlapAsync(UpdateWhiteboardDto dto, Whiteboard existingWhiteboard)
    {
        // Get existing components in the learning space for overlap checking
        var existingComponents = await _whiteboardRepository.GetByLearningSpaceIdAsync(existingWhiteboard.LearningSpaceId);
        
        // Check for overlap with other components (excluding the current whiteboard)
        foreach (var component in existingComponents)
        {
            if (component.ComponentId != dto.WhiteboardId && IsOverlapping(dto.X, dto.Z, dto.Width, dto.Depth, component))
            {
                return UpdateWhiteboardResult.Failure("Position overlaps with existing component");
            }
        }
        return null;
    }

    private async Task<UpdateWhiteboardResult> PerformUpdateAsync(UpdateWhiteboardDto dto, Whiteboard existingWhiteboard)
    {
        try
        {
            // Update the whiteboard properties
            existingWhiteboard.Update(
                dto.Width,
                dto.Height,
                dto.Depth,
                dto.X,
                dto.Y,
                dto.Z,
                dto.Orientation,
                dto.MarkerColor);

            await _whiteboardRepository.UpdateAsync(existingWhiteboard);
            return UpdateWhiteboardResult.Success(existingWhiteboard);
        }
        catch (ArgumentException)
        {
            return UpdateWhiteboardResult.Failure("Invalid marker color");
        }
        catch (InvalidOperationException ex)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }
    }

    private static bool IsOverlapping(float x, float z, float width, float depth, LearningComponent component)
    {
        // Check if two rectangles overlap in 2D space (X-Z plane)
        bool xOverlap = x < component.X + component.Width && x + width > component.X;
        bool zOverlap = z < component.Z + component.Depth && z + depth > component.Z;
        return xOverlap && zOverlap;
    }
}
