using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component IDs with a COMP- prefix.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A unique component ID in the format COMP-{GUID prefix}.</returns>
    public Task<string> GenerateIdAsync()
    {
        return Task.FromResult($"COMP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}");
    }
}
