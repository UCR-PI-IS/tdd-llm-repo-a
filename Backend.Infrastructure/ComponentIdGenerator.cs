using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component identifiers using GUIDs with a "COMP-" prefix.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique identifier for a component.
    /// </summary>
    /// <returns>A unique identifier string starting with "COMP-".</returns>
    public Task<string> GenerateIdAsync()
    {
        return Task.FromResult($"COMP-{Guid.NewGuid()}");
    }
}
