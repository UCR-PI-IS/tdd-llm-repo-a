using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component IDs with a COMP- prefix.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A unique identifier string prefixed with "COMP-".</returns>
    public Task<string> GenerateIdAsync()
    {
        return Task.FromResult($"COMP-{Guid.NewGuid():N}");
    }
}
