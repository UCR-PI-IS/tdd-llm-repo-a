using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Validates <see cref="CreateComponentRequest"/> instances.
/// </summary>
internal static class ComponentRequestValidator
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    /// <summary>
    /// Validates the given request, throwing <see cref="ValidationException"/> on any rule violation.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    public static void Validate(CreateComponentRequest request)
    {
        ValidatePositiveDimension(request.Width, "Width");
        ValidateNonNegativeDimension(request.Height, "Height");
        ValidateNonNegativeDimension(request.Depth, "Depth");
        ValidateNonNegativeDimension(request.X, "X");
        ValidateNonNegativeDimension(request.Y, "Y");
        ValidateNonNegativeDimension(request.Z, "Z");

        if (!ValidOrientations.Contains(request.Orientation))
            throw new ValidationException("Orientation must be one of: North, South, East, West.");
    }

    private static void ValidatePositiveDimension(float value, string name)
    {
        if (value <= 0f)
            throw new ValidationException($"{name} must be positive.");
    }

    private static void ValidateNonNegativeDimension(float value, string name)
    {
        if (value < 0f)
            throw new ValidationException($"{name} cannot be negative.");
    }
}
