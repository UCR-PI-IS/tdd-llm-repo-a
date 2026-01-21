using System;
using System.Collections.Generic;
using System.Linq;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public string id { get; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public string type { get; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float height { get; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float width { get; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float length { get; }

    private readonly List<LearningComponent> _components = new List<LearningComponent>();

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    public LearningSpace(string id, string type, float height, float width, float length)
    {
        this.id = id;
        this.type = type;
        this.height = height;
        this.width = width;
        this.length = length;
    }

    /// <summary>
    /// Adds a learning component to the learning space.
    /// </summary>
    public void AddComponent(LearningComponent component)
    {
        if(component == null) throw new ArgumentNullException(nameof(component));
        _components.Add(component);
    }

    /// <summary>
    /// Lists all components in the learning space.
    /// </summary>
    public List<LearningComponent> ListComponents()
    {
        return _components.ToList();
    }
}
