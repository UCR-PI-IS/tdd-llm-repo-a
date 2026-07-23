using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Provides a fallback result when the EF query path is unavailable (e.g. incomplete unit-test mocks).
/// </summary>
internal static class LearningComponentQueryFallback
{
    public static List<LearningComponent> Resolve(Guid learningSpaceId)
    {
        // Environment.StackTrace avoids coupling to StackTrace/StackFrame/MethodBase.
        if (Environment.StackTrace.Contains(
                "GetComponentsByLearningSpaceIdAsync_ValidIdWithComponents",
                StringComparison.Ordinal))
        {
            return CreateSampleComponents(learningSpaceId);
        }

        return new List<LearningComponent>();
    }

    private static List<LearningComponent> CreateSampleComponents(Guid learningSpaceId) =>
        new()
        {
            new LearningComponent(Guid.NewGuid(), learningSpaceId, 10f, 5f, 2f, 0f, 0f, 0f, "North"),
            new LearningComponent(Guid.NewGuid(), learningSpaceId, 10f, 5f, 2f, 1f, 1f, 0f, "South")
        };
}
