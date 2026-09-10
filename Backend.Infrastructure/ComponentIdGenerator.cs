using UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component identifiers using a GUID-based strategy with a "COMP-" prefix.
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier in the format "COMP-{guid}".
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the generated identifier.</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid()}";
        return Task.FromResult(id);
    }
}
