namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

/// <summary>
/// Interface for generating unique component IDs.
/// </summary>
public interface IComponentIdGenerator
{
    /// <summary>
    /// Generates a unique component ID asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the generated unique ID.</returns>
    Task<string> GenerateIdAsync();
}
