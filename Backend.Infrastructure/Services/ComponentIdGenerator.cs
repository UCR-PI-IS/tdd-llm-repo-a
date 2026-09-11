using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Services;

/// <summary>
/// Generates unique component IDs with the "COMP-" prefix.
/// </summary>
internal class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a new unique component ID in the format COMP-XXXXX.
    /// </summary>
    /// <returns>A unique component ID string.</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        return Task.FromResult(id);
    }
}
