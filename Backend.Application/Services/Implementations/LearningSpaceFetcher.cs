using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Fetches a learning space by ID and validates its existence.
/// </summary>
internal static class LearningSpaceFetcher
{
    /// <summary>
    /// Fetches a learning space by ID.
    /// </summary>
    /// <exception cref="NotFoundException">Thrown when the learning space is not found.</exception>
    public static async Task<LearningSpace> FetchAsync(
        ILearningSpaceRepository repository,
        string learningSpaceId)
    {
        var learningSpace = await repository.GetByIdAsync(learningSpaceId);
        if (learningSpace == null)
        {
            throw new NotFoundException("Learning space not found: " + learningSpaceId);
        }

        return learningSpace;
    }
}
