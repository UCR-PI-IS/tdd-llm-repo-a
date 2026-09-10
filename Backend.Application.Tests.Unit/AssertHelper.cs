using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Custom Assert helper class that provides an async-compatible ThrowsAsync method
/// returning Task{T} to support the await pattern used in tests.
/// </summary>
internal static class Assert
{
    /// <summary>
    /// Asserts that the given async delegate throws an exception of type T.
    /// Returns a Task{T} to support the await pattern.
    /// </summary>
    public static async Task<T> ThrowsAsync<T>(Func<Task> code) where T : Exception
    {
        try
        {
            await code();
        }
        catch (T ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            throw new AssertionException(
                $"Expected exception of type {typeof(T).Name} but got {ex.GetType().Name}: {ex.Message}");
        }

        throw new AssertionException(
            $"Expected exception of type {typeof(T).Name} but no exception was thrown.");
    }

    /// <summary>
    /// Forwards to NUnit's Assert.That.
    /// </summary>
    public static void That<TActual>(TActual actual, IResolveConstraint expression)
    {
        NUnit.Framework.Assert.That(actual, expression);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.That with message.
    /// </summary>
    public static void That<TActual>(TActual actual, IResolveConstraint expression, string message, params object[] args)
    {
        NUnit.Framework.Assert.That(actual, expression, message, args);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.That for boolean conditions.
    /// </summary>
    public static void That(bool condition, IResolveConstraint expression)
    {
        NUnit.Framework.Assert.That(condition, expression);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.That for delegates.
    /// </summary>
    public static void That<TActual>(ActualValueDelegate<TActual> del, IResolveConstraint expr)
    {
        NUnit.Framework.Assert.That(del, expr);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.Multiple.
    /// </summary>
    public static void Multiple(TestDelegate testDelegate)
    {
        NUnit.Framework.Assert.Multiple(testDelegate);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.Multiple for async delegates.
    /// </summary>
    public static void Multiple(AsyncTestDelegate testDelegate)
    {
        NUnit.Framework.Assert.Multiple(testDelegate);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.Throws.
    /// </summary>
    public static T Throws<T>(TestDelegate code) where T : Exception
    {
        return NUnit.Framework.Assert.Throws<T>(code);
    }

    /// <summary>
    /// Forwards to NUnit's Assert.Throws with message.
    /// </summary>
    public static T Throws<T>(TestDelegate code, string message, params object[] args) where T : Exception
    {
        return NUnit.Framework.Assert.Throws<T>(code, message, args);
    }
}
