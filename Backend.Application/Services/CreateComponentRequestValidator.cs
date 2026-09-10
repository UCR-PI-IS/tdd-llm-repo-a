using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Validates <see cref="CreateComponentRequest"/> data before component creation.
/// </summary>
internal static class CreateComponentRequestValidator
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    /// <summary>
    /// Validates all fields in the creation request.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <exception cref="ValidationException">Thrown when any field is invalid.</exception>
    public static void Validate(CreateComponentRequest request)
    {
        ValidateDimensions(request);
        ValidateCoordinates(request);
        ValidateOrientation(request.Orientation);
    }

    /// <summary>
    /// Validates that a learning space ID is not null or empty.
    /// </summary>
    /// <param name="learningSpaceId">The learning space ID to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the ID is null or empty.</exception>
    public static void ValidateLearningSpaceId(string learningSpaceId)
    {
        if (string.IsNullOrEmpty(learningSpaceId))
            throw new ArgumentException("Learning space ID cannot be null or empty.", nameof(learningSpaceId));
    }

    private static void ValidateDimensions(CreateComponentRequest request)
    {
        if (request.Width <= 0f)
            throw new ValidationException("Width must be positive.");

        if (request.Height <= 0f)
            throw new ValidationException("Height must be positive.");

        if (request.Depth <= 0f)
            throw new ValidationException("Depth must be positive.");
    }

    private static void ValidateCoordinates(CreateComponentRequest request)
    {
        if (request.X < 0f)
            throw new ValidationException("X coordinate cannot be negative.");

        if (request.Y < 0f)
            throw new ValidationException("Y coordinate cannot be negative.");

        if (request.Z < 0f)
            throw new ValidationException("Z coordinate cannot be negative.");
    }

    private static void ValidateOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ValidationException(
                $"Orientation '{orientation}' is invalid. Must be one of: North, South, East, West.");
    }
}
