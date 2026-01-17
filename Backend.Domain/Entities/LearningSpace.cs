using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities
{
    /// <summary>
    /// Represents a learning space in a building of the theme park UCR.
    /// </summary>
    public class LearningSpace
    {
        private readonly List<LearningComponent> _components = new List<LearningComponent>();
        public string id { get; }
        public string type { get; }
        public float height { get; }
        public float width { get; }
        public float length { get; }

        public LearningSpace() {}
        public LearningSpace(string id, string type, float height, float width, float length)
        {
            this.id = id;
            this.type = type;
            this.height = height;
            this.width = width;
            this.length = length;
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
}
