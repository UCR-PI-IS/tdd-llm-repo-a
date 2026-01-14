using NUnit.Framework;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities.Spaces
{
    [TestFixture]
    public class LearningSpaceComponentsTests
    {
        [Test]
        public void ListComponents_ReturnsAllComponents_WhenAvailable()
        {
            var components = new List<LearningComponent> { new Whiteboard(), new Projector() };
            var learningSpace = new LearningSpace(components);
            var listedComponents = learningSpace.ListComponents();
            Assert.AreEqual(2, listedComponents.Count);
            Assert.Contains(components[0], listedComponents);
            Assert.Contains(components[1], listedComponents);
        }

        [Test]
        public void ListComponents_ReturnsEmptyList_WhenNoComponents()
        {
            var learningSpace = new LearningSpace(new List<LearningComponent>());
            var listedComponents = learningSpace.ListComponents();
            Assert.IsEmpty(listedComponents);
        }

        [Test]
        public void ListComponents_ThrowsDomainException_WhenLearningSpaceIsInvalid()
        {
            LearningSpace invalidLearningSpace = null;
            Assert.Throws<DomainException>(() => invalidLearningSpace.ListComponents());
        }
    }
}
