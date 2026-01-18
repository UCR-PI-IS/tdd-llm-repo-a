using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit
{
    [TestFixture]
    public class LearningSpaceDomainTests
    {
        private LearningSpace learningSpaceWithComponents;
        private LearningSpace learningSpaceWithoutComponents;

        [SetUp]
        public void Setup()
        {
            learningSpaceWithComponents = new LearningSpace("id", "type", 0, 0, 0);
            learningSpaceWithComponents.AddComponent(new LearningComponent("Whiteboard"));
            learningSpaceWithComponents.AddComponent(new LearningComponent("Projector"));

            learningSpaceWithoutComponents = new LearningSpace("id2", "type", 0, 0, 0);
        }

        [Test(Description = "Returns all components when learning space has components")]
        public void ListComponents_WhenLearningSpaceHasComponents_ReturnsAllComponents()
        {
            var components = learningSpaceWithComponents.ListComponents();
            Assert.AreEqual(2, components.Count);
            Assert.IsTrue(components.Any(c => c.Name == "Whiteboard"));
            Assert.IsTrue(components.Any(c => c.Name == "Projector"));
        }

        [Test(Description = "Returns empty list when learning space has no components")]
        public void ListComponents_WhenLearningSpaceHasNoComponents_ReturnsEmptyList()
        {
            var components = learningSpaceWithoutComponents.ListComponents();
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test(Description = "Throws ArgumentNullException when learning space is invalid (null)")]
        public void ListComponents_WhenLearningSpaceIsInvalid_ThrowsException()
        {
            LearningSpace invalidLearningSpace = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                var components = invalidLearningSpace.ListComponents();
            });
        }
    }
}
