using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Services;

/// <summary>
/// Generates unique component identifiers with a "COMP-" prefix.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A unique component identifier prefixed with "COMP-".</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid().ToString("N")[^10..]}";
        return Task.FromResult(id);
    }
}
