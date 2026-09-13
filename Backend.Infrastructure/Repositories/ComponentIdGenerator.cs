using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

/// <summary>
/// Generates unique component IDs in the format COMP-#####.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    private static int _counter = 0;

    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A task containing the generated ID.</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Interlocked.Increment(ref _counter):D5}";
        return Task.FromResult(id);
    }
}
