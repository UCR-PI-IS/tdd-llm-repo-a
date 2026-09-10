namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Services;

/// <summary>
/// Service contract for generating unique component IDs.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component identifier asynchronously.
    /// </summary>
    /// <returns>A unique component ID string.</returns>
    Task<string> GenerateIdAsync();
}
