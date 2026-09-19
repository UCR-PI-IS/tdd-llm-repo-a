namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Validates spatial positions and boundaries for learning components.
/// </summary>
public static class PositionValidator
{
    /// <summary>
    /// Validates that a component's position does not exceed learning space boundaries.
    /// </summary>
    public static void ValidateBoundary(
        float x, float z, float width, float depth,
        float learningSpaceWidth, float learningSpaceLength)
    {
        if (x + width > learningSpaceWidth)
            throw new InvalidOperationException("Position exceeds learning space boundaries");
        if (z + depth > learningSpaceLength)
            throw new InvalidOperationException("Position exceeds learning space boundaries");
    }

    /// <summary>
    /// Validates that a component's position does not overlap with existing components.
    /// </summary>
    public static void ValidateNoOverlap(
        string componentId,
        float x, float z, float width, float depth,
        IEnumerable<LearningComponent> existingComponents)
    {
        foreach (var component in existingComponents)
        {
            if (component.ComponentId == componentId)
                continue;
            if (x < component.X + component.Width && x + width > component.X
                && z < component.Z + component.Depth && z + depth > component.Z)
                throw new InvalidOperationException("Position overlaps with existing component");
        }
    }
}
