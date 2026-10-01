namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Provides shared validation helpers for <see cref="Building"/> properties.
/// </summary>
internal static class BuildingValidation
{
    /// <summary>
    /// Validates that a string value is not null or empty.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when the value is null or empty.</exception>
    public static void ValidateRequiredString(string value, string paramName)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
    }

    /// <summary>
    /// Validates that a numeric value is strictly positive.
    /// </summary>
    /// <param name="value">The numeric value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static void ValidatePositive(float value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }

    /// <summary>
    /// Validates that an integer value is strictly positive.
    /// </summary>
    /// <param name="value">The integer value to validate.</param>
    /// <param name="paramName">The name of the parameter being validated.</param>
    /// <exception cref="ArgumentException">Thrown when the value is zero or negative.</exception>
    public static void ValidatePositive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive", paramName);
    }
}
