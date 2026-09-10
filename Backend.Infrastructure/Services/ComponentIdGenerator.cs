using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique identifiers for learning components using GUIDs with a "COMP-" prefix.
/// </summary>
internal class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique identifier for a learning component.
    /// </summary>
    /// <returns>A unique identifier string in the format "COMP-{guid}".</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid():N}";
        return Task.FromResult(id);
    }
}
