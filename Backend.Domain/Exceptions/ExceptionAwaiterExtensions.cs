using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

/// <summary>
/// Provides a GetAwaiter extension for Exception so that NUnit's ThrowsAsync
/// (which returns the exception directly in NUnit 3.14) can be used with await.
/// </summary>
public static class ExceptionAwaiterExtensions
{
    /// <summary>
    /// Returns a TaskAwaiter for an exception, enabling await on NUnit ThrowsAsync results.
    /// </summary>
    public static TaskAwaiter<T> GetAwaiter<T>(this T exception) where T : Exception
    {
        return Task.FromResult(exception).GetAwaiter();
    }
}
