using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in the ThemePark UCR, e.g., classroom, lab, auditorium.
/// Supports adding and listing learning components.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public string id { get; }
    public string type { get; }
    public float height { get; }
    public float width { get; }
    public float length { get; }

    // Internal list of components
    private readonly List<LearningComponent> _components = new();

    public LearningSpace() {}

    public LearningSpace(string id, string type, float height, float width, float length)
    {
        this.id = id;
        this.type = type;
        this.height = height;
        this.width = width;
        this.length = length;
    }

    /// <summary>
    /// Adds a learning component to the space.
    /// </summary>
    public void AddComponent(LearningComponent component)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        _components.Add(component);
    }

    /// <summary>
    /// Lists all components in this space. Returns empty list if none.
    /// </summary>
    public List<LearningComponent> ListComponents()
    {
        return new List<LearningComponent>(_components);
    }
}
