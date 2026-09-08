using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component identifiers with the "COMP-" prefix.
/// </summary>
internal class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A unique component identifier string prefixed with "COMP-".</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid()}";
        return Task.FromResult(id);
    }
}
