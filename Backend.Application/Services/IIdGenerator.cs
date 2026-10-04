namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Interface for generating unique identifiers for components.
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// Generates a unique identifier asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation, containing the generated unique identifier.</returns>
    Task<string> GenerateIdAsync();
}
