using UCR.ECCI.PI.ThemePark.Backend.Application.ViewModels;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service implementation for managing learning components.
/// </summary>
public class LearningComponentService : ILearningComponentService
{
    private readonly ILearningComponentRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LearningComponentService"/> class.
    /// </summary>
    /// <param name="repository">The learning component repository.</param>
    public LearningComponentService(ILearningComponentRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<LearningComponentViewModel>> GetComponentsByLearningSpaceIdAsync(string learningSpaceId)
    {
        var components = await _repository.GetByLearningSpaceIdAsync(learningSpaceId);
        
        return components.Select(c => new LearningComponentViewModel
        {
            ComponentId = c.ComponentId,
            LearningSpaceId = c.LearningSpaceId,
            Width = c.Width,
            Height = c.Height,
            Depth = c.Depth,
            X = c.X,
            Y = c.Y,
            Z = c.Z,
            Orientation = c.Orientation
        });
    }
}
