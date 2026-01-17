using System;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities
{
    public class LearningComponent
    {
        public string Name { get; }

        public LearningComponent(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
        }
    }
}
