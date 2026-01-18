using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// In-memory (fake) implementation for TDD to pass tests for component listing.
/// </summary>
internal class SqlLearningSpaceListRepository : ILearningSpaceListRepository
{
    // Minimal in-memory simulation of test data.
    private static readonly Dictionary<int, List<LearningComponent>> _componentsBySpaceId = new()
    {
        { 1, new List<LearningComponent> { new LearningComponent("Whiteboard"), new LearningComponent("Projector") } },
        { 2, new List<LearningComponent>() }
    };

    public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
    {
        if (!_componentsBySpaceId.ContainsKey(learningSpaceId))
            throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
        return new List<LearningComponent>(_componentsBySpaceId[learningSpaceId]);
    }

    // Existing dummy implementations to satisfy interface
    public Task<LearningSpace> GetCurrentLearningSpaceListAsync() => Task.FromResult<LearningSpace>(null);
    public Task<List<LearningSpace>> GetAllLearningSpacesAsync() => Task.FromResult(new List<LearningSpace>());
}
