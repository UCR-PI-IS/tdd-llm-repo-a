namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

using System;

/// <summary>
/// Represents a learning component in a learning space.
/// </summary>
public class LearningComponent
{
    private static readonly string[] ValidOrientations = { "North", "South", "East", "West" };

    public Guid ComponentId { get; }
    public Guid LearningSpaceId { get; }
    public float Width { get; }
    public float Height { get; }
    public float Depth { get; }
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    public string Orientation { get; }

    public LearningComponent(Guid componentId, Guid learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        EnsureNonNegative(width, "width", "Width");
        EnsureNonNegative(height, "height", "Height");
        EnsureNonNegative(depth, "depth", "Depth");
        EnsureNonNegative(x, "x", "X");
        EnsureNonNegative(y, "y", "Y");
        EnsureNonNegative(z, "z", "Z");
        EnsureValidOrientation(orientation);

        ComponentId = componentId;
        LearningSpaceId = learningSpaceId;
        Width = width;
        Height = height;
        Depth = depth;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

    private static void EnsureNonNegative(float value, string paramName, string displayName)
    {
        if (value < 0)
            throw new ArgumentException($"{displayName} cannot be negative.", paramName);
    }

    private static void EnsureValidOrientation(string orientation)
    {
        if (!ValidOrientations.Contains(orientation))
            throw new ArgumentException("Orientation must be one of: North, South, East, West.", "orientation");
    }
}
