namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Provides spatial validation helpers for checking axis fit and overlap
/// between learning components within a learning space.
/// </summary>
internal static class SpatialValidator
{
    /// <summary>
    /// Determines whether a single axis (size + position) fits within a space dimension.
    /// </summary>
    public static bool FitsAxis(float size, float position, float spaceSize)
    {
        bool fitsSize = size <= spaceSize;
        bool fitsPosition = position + size <= spaceSize;
        return fitsSize & fitsPosition;
    }

    /// <summary>
    /// Determines whether a component with the given dimensions and position
    /// overlaps with another existing component.
    /// </summary>
    public static bool Overlaps(
        float x, float y, float z,
        float width, float height, float depth,
        LearningComponent other)
    {
        bool overlapX = x < other.X + other.Width & x + width > other.X;
        bool overlapY = y < other.Y + other.Height & y + height > other.Y;
        bool overlapZ = z < other.Z + other.Depth & z + depth > other.Z;
        return overlapX & overlapY & overlapZ;
    }

    /// <summary>
    /// Throws <see cref="InvalidOperationException"/> if the proposed position
    /// overlaps with any component in the provided list (excluding the component
    /// identified by <paramref name="componentId"/>).
    /// </summary>
    public static void ThrowIfOverlaps(
        float x, float y, float z,
        float width, float height, float depth,
        string componentId,
        List<LearningComponent> existingComponents)
    {
        foreach (var component in existingComponents)
        {
            if (component.ComponentId == componentId) continue;
            if (Overlaps(x, y, z, width, height, depth, component))
                throw new InvalidOperationException("Position overlaps with existing component");
        }
    }
}
