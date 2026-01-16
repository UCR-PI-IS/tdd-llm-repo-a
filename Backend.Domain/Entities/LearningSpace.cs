namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents a learning space in a building of the theme park UCR.
/// </summary>
public class LearningSpace
{
    /// <summary>
    /// Unique identifier for the learning space.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Type of the learning space (e.g., classroom, lab and auditorium).
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Height of the learning space in meters.
    /// </summary>
    public float Height { get; }

    /// <summary>
    /// Width of the learning space in meters.
    /// </summary>
    public float Width { get; }

    /// <summary>
    /// Length of the learning space in meters.
    /// </summary>
    public float Length { get; }

    private readonly List<LearningComponent> _components = new();

    /// <summary>
    /// Constructor for the LearningSpace class.
    /// </summary>
    /// <param name="id">Unique identifier for the learning space</param>
    /// <param name="type">Type of the learning space</param>
    /// <param name="height">Height of the learning space in meters</param>
    /// <param name="width">Width of the learning space in meters</param>
    /// <param name="length">Length of the learning space in meters</param>
    public LearningSpace(string id, string type, float height, float width, float length)
    {
        Id = id;
        Type = type;
        Height = height;
        Width = width;
        Length = length;
    }

    /// <summary>
    /// Adds a component to the learning space.
    /// </summary>
    /// <param name="component">Component to add</param>
    public void AddComponent(LearningComponent component)
    {
        if (component == null) throw new ArgumentNullException(nameof(component));
        _components.Add(component);
    }

    /// <summary>
    /// Lists all components in the learning space.
    /// </summary>
    /// <returns>List of components</returns>
    public IReadOnlyList<LearningComponent> ListComponents()
    {
        return _components.AsReadOnly();
    }
}

// Class for LearningComponent entity (if it does not exist yet)
public class LearningComponent
{
    public string Name { get; }

    public LearningComponent(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
