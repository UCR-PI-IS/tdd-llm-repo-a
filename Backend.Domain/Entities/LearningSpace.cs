using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities
{
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
        /// Type of the learning space (e.g., classroom, lab, auditorium).
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

        public LearningSpace() { }

        public LearningSpace(string id, string type, float height, float width, float length)
        {
            Id = id;
            Type = type;
            Height = height;
            Width = width;
            Length = length;
        }

        public void AddComponent(LearningComponent component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            _components.Add(component);
        }

        public List<LearningComponent> ListComponents()
        {
            return new List<LearningComponent>(_components);
        }
    }

    public class LearningComponent
    {
        public string Name { get; }
        public LearningComponent(string name)
        {
            Name = name;
        }
    }
}
