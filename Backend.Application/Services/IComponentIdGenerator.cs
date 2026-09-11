namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for generating unique component IDs.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a new unique component ID.
    /// </summary>
    /// <returns>A unique component ID string.</returns>
    Task<string> GenerateIdAsync();
}
