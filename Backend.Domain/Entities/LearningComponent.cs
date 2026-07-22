using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

public class LearningComponent
{
    public string ComponentId { get; }
    public string LearningSpaceId { get; }
    public float Width { get; }
    public float Height { get; }
    public float Depth { get; }
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    public string Orientation { get; }

    public LearningComponent(string componentId, string learningSpaceId, float width, float height, float depth, float x, float y, float z, string orientation)
    {
        if (width < 0) throw new ArgumentException("Width cannot be negative.", nameof(width));
        if (height < 0) throw new ArgumentException("Height cannot be negative.", nameof(height));
        if (depth < 0) throw new ArgumentException("Depth cannot be negative.", nameof(depth));
        if (x < 0) throw new ArgumentException("X cannot be negative.", nameof(x));
        if (y < 0) throw new ArgumentException("Y cannot be negative.", nameof(y));
        if (z < 0) throw new ArgumentException("Z cannot be negative.", nameof(z));
        if (string.IsNullOrWhiteSpace(orientation) || !IsValidOrientation(orientation))
            throw new ArgumentException("Orientation must be one of: North, South, East, West.", nameof(orientation));

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

    private static bool IsValidOrientation(string orientation)
    {
        return orientation == "North" || orientation == "South" || orientation == "East" || orientation == "West";
    }
}
