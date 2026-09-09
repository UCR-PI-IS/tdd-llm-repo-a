namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Generates unique component identifiers using GUIDs with a "COMP-" prefix.
/// </summary>
internal class ComponentIdGenerator : IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier in the format "COMP-{guid}".
    /// </summary>
    /// <returns>A unique identifier string starting with "COMP-".</returns>
    public Task<string> GenerateIdAsync()
    {
        var id = $"COMP-{Guid.NewGuid()}";
        return Task.FromResult(id);
    }
}
