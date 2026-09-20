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
    private readonly ILearningSpaceReadRepository _learningSpaceReadRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhiteboardService"/> class.
    /// </summary>
    /// <param name="whiteboardRepository">The whiteboard repository dependency.</param>
    /// <param name="learningSpaceReadRepository">The learning space read repository dependency.</param>
    public WhiteboardService(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceReadRepository learningSpaceReadRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceReadRepository = learningSpaceReadRepository;
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
        var learningSpace = await _learningSpaceReadRepository.GetByIdAsync(request.LearningSpaceId);
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
    /// Updates an existing whiteboard with the specified parameters.
    /// Validates existence, marker color, boundary constraints, and overlap constraints.
    /// </summary>
    /// <param name="dto">The update data transfer object containing the new values.</param>
    /// <returns>A result indicating success or failure of the update operation.</returns>
    public async Task<UpdateWhiteboardResult> UpdateWhiteboardAsync(UpdateWhiteboardDto dto)
    {
        var existingWhiteboard = await _whiteboardRepository.GetByIdAsync(dto.ComponentId);
        if (existingWhiteboard == null)
            return UpdateWhiteboardResult.Failure("Whiteboard not found");

        if (string.IsNullOrEmpty(dto.MarkerColor))
            return UpdateWhiteboardResult.Failure("Invalid marker color");

        try
        {
            var spatialError = await ValidateSpatialConstraintsAsync(dto, existingWhiteboard);
            if (spatialError != null)
                return UpdateWhiteboardResult.Failure(spatialError);

            existingWhiteboard.Update(
                dto.Width, dto.Height, dto.Depth,
                dto.X, dto.Y, dto.Z,
                dto.Orientation, dto.MarkerColor);

            await _whiteboardRepository.UpdateAsync(existingWhiteboard);
            return UpdateWhiteboardResult.Success(existingWhiteboard);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return UpdateWhiteboardResult.Failure(ex.Message);
        }
    }

    private async Task<string?> ValidateSpatialConstraintsAsync(UpdateWhiteboardDto dto, Whiteboard existingWhiteboard)
    {
        var learningSpace = await _learningSpaceReadRepository.GetByIdAsync(existingWhiteboard.LearningSpaceId);
        if (learningSpace == null)
            return null;

        if (dto.X + dto.Width > learningSpace.Width || dto.Z + dto.Depth > learningSpace.Length)
            return "Position exceeds learning space boundaries";

        var existingComponents = await _whiteboardRepository.GetByLearningSpaceIdAsync(existingWhiteboard.LearningSpaceId);
        foreach (var component in existingComponents)
        {
            if (component.ComponentId == dto.ComponentId)
                continue;

            if (Whiteboard.Overlaps(dto.X, dto.Z, dto.Width, dto.Depth, component.X, component.Z, component.Width, component.Depth))
                return "Position overlaps with existing component";
        }

        return null;
    }
}
