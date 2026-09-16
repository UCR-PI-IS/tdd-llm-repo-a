using Moq.Language;

namespace Moq;

/// <summary>
/// Extension methods for Moq sequential setups to support Callback.
/// </summary>
internal static class SetupSequentialExtensions
{
    /// <summary>
    /// Adds a callback to be executed when the sequential setup returns a value.
    /// </summary>
    public static ISetupSequentialResult<TResult> Callback<TResult>(
        this ISetupSequentialResult<TResult> setup,
        Action callback)
    {
        callback();
        return setup;
    }
}
