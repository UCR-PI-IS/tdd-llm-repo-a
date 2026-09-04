using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Encapsulates the whiteboard creation workflow.
/// </summary>
internal class CreateWhiteboardUseCase
{
    private readonly IWhiteboardRepository _whiteboardRepository;
    private readonly ILearningSpaceRepository _learningSpaceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWhiteboardUseCase"/> class.
    /// </summary>
    public CreateWhiteboardUseCase(
        IWhiteboardRepository whiteboardRepository,
        ILearningSpaceRepository learningSpaceRepository)
    {
        _whiteboardRepository = whiteboardRepository;
        _learningSpaceRepository = learningSpaceRepository;
    }

    /// <summary>
    /// Executes the whiteboard creation workflow.
    /// </summary>
    public async Task<Whiteboard> ExecuteAsync(CreateWhiteboardRequest request)
    {
        var learningSpace = await LearningSpaceFetcher.FetchAsync(_learningSpaceRepository, request.LearningSpaceId);
        var whiteboard = WhiteboardBuilder.Build(request, learningSpace);
        await WhiteboardPersistence.SaveAsync(_whiteboardRepository, whiteboard);
        return whiteboard;
    }
}
