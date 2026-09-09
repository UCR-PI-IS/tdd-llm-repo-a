namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Contract for generating unique identifiers for components.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique identifier for a component.
    /// </summary>
    /// <returns>A unique identifier string.</returns>
    Task<string> GenerateIdAsync();
}
