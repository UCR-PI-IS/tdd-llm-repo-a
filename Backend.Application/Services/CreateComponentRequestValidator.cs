using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

internal static class CreateComponentRequestValidator
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    public static void Validate(CreateComponentRequest request)
    {
        if (request.Width <= 0f)
            throw new ValidationException("Width must be positive");
        if (request.Height <= 0f)
            throw new ValidationException("Height must be positive");
        if (request.Depth <= 0f)
            throw new ValidationException("Depth must be positive");
        if (request.X < 0f)
            throw new ValidationException("X coordinate cannot be negative");
        if (request.Y < 0f)
            throw new ValidationException("Y coordinate cannot be negative");
        if (request.Z < 0f)
            throw new ValidationException("Z coordinate cannot be negative");
        if (!ValidOrientations.Contains(request.Orientation))
            throw new ValidationException("Orientation must be one of: North, South, East, West");
    }
}
