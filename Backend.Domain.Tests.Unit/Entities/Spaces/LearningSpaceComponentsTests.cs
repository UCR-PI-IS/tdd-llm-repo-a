using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities.Spaces
{
    [TestFixture]
    public class LearningSpaceComponentsTests
    {
        [Test]
        public void ListComponents_ShouldReturnAllComponents_WhenAvailable()
        {
            var components = new List<LearningComponent> { new Whiteboard(), new Projector() };
            var learningSpace = new LearningSpace(components);
            var listedComponents = learningSpace.ListComponents();
            Assert.AreEqual(2, listedComponents.Count);
            Assert.Contains(components[0], listedComponents);
            Assert.Contains(components[1], listedComponents);
        }

        [Test]
        public void ListComponents_ShouldReturnEmptyList_WhenNoComponents()
        {
            var learningSpace = new LearningSpace(new List<LearningComponent>());
            var listedComponents = learningSpace.ListComponents();
            Assert.IsEmpty(listedComponents);
        }

        [Test]
        public void ListComponents_ShouldThrowDomainException_WhenLearningSpaceIsInvalid()
        {
            LearningSpace invalidLearningSpace = null;
            Assert.Throws<DomainException>(() => invalidLearningSpace.ListComponents());
        }
    }

    // Dummy classes for compilation - replace with actual domain classes
    public class LearningSpace
    {
        private readonly List<LearningComponent> _components;
        public LearningSpace(List<LearningComponent> components) => _components = components ?? throw new DomainException("Invalid LearningSpace");
        public List<LearningComponent> ListComponents() => _components;
    }

    public class LearningComponent { }
    public class Whiteboard : LearningComponent { }
    public class Projector : LearningComponent { }

    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
