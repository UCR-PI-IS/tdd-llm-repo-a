namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Contract for generating unique identifiers for learning components.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique identifier for a learning component.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    Task<string> GenerateIdAsync();
}
