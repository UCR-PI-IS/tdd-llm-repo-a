namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Validation;

/// <summary>
/// Provides guard clauses for common validation scenarios.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided string is null or empty.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null or empty.</exception>
    public static void AgainstNullOrEmpty(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided float value is not positive.
    /// </summary>
    /// <param name="value">The float value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is zero or negative.</exception>
    public static void AgainstNonPositive(float value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided int value is not positive.
    /// </summary>
    /// <param name="value">The int value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is zero or negative.</exception>
    public static void AgainstNonPositive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }
}
