using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure;

/// <summary>
/// Generates unique component IDs with the format "COMP-{GUID}".
/// </summary>
public class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated unique ID in the format "COMP-{GUID}".</returns>
    public Task<string> GenerateIdAsync()
    {
        var guid = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        var generatedId = $"COMP-{guid}";
        return Task.FromResult(generatedId);
    }
}
