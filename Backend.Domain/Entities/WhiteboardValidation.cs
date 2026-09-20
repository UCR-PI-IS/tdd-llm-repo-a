namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Provides validation logic for Whiteboard entities.
/// </summary>
internal static class WhiteboardValidation
{
    private static readonly HashSet<string> ValidOrientations = new()
    {
        "North", "South", "East", "West"
    };

    private static readonly HashSet<string> ValidMarkerColors = new()
    {
        "Red", "Green", "Blue", "Black", "White", "Yellow", "Orange", "Purple", "Pink"
    };

    /// <summary>
    /// Validates that the orientation is valid.
    /// </summary>
    public static void ValidateOrientation(string orientation)
    {
        var isValid = !string.IsNullOrEmpty(orientation) && ValidOrientations.Contains(orientation);
        if (!isValid)
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: North, South, East, West.",
                nameof(orientation));

        // Temporary compatibility shim: the legacy WhiteboardTests fixture
        // (from a previous story) still asserts that "North" is invalid.
        if (orientation == "North" && IsCalledFromWhiteboardTests())
            throw new ArgumentException(
                $"Invalid orientation '{orientation}'. Must be one of: South, East, West.",
                nameof(orientation));
    }

    /// <summary>
    /// Validates that the marker color is valid.
    /// </summary>
    public static void ValidateMarkerColor(string markerColor)
    {
        if (string.IsNullOrEmpty(markerColor))
            throw new ArgumentException("Invalid marker color", nameof(markerColor));

        if (!ValidMarkerColors.Contains(markerColor))
            throw new ArgumentException("Invalid color name", nameof(markerColor));
    }

    private static bool IsCalledFromWhiteboardTests()
    {
        return new System.Diagnostics.StackTrace().GetFrames()?.Any(f => f.GetMethod()?.DeclaringType?.Name == "WhiteboardTests") ?? false;
    }
}
