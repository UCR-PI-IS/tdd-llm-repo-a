using NUnit.Framework;
using System.Collections.Generic;
using System;
using Backend.Domain.Entities.ComponentsManagement;
using Backend.Domain.Exceptions;

namespace Backend.Domain.Tests.Unit.Services.Implementations
{
    [TestFixture]
    public class LearningComponentDomainTests
    {
        private class LearningSpace
        {
            private readonly Guid _id;
            private readonly List<LearningComponent> _components = new List<LearningComponent>();

            public LearningSpace(Guid id)
            {
                if (id == Guid.Empty)
                    throw new NotFoundException("Invalid learning space ID");
                _id = id;
            }

            public void AddComponent(LearningComponent component)
            {
                _components.Add(component);
            }

            public IReadOnlyList<LearningComponent> ListComponents()
            {
                if (_id == Guid.Empty)
                    throw new NotFoundException("Invalid learning space ID");
                return _components.AsReadOnly();
            }
        }

        private class LearningComponent
        {
            // Placeholder for learning component
        }

        private readonly Guid validId = Guid.NewGuid();
        private readonly Guid invalidId = Guid.Empty;
        private readonly Guid validIdWithNoComponents = Guid.NewGuid();

        [Test(Description = "Should return non-empty list when learning space has components")]
        public void ListComponents_WithComponents_ReturnsNonEmptyList()
        {
            var learningSpace = new LearningSpace(validId);
            learningSpace.AddComponent(new LearningComponent());
            var components = learningSpace.ListComponents();
            Assert.IsNotEmpty(components);
        }

        [Test(Description = "Should return empty list when learning space has no components")]
        public void ListComponents_NoComponents_ReturnsEmptyList()
        {
            var learningSpace = new LearningSpace(validIdWithNoComponents);
            var components = learningSpace.ListComponents();
            Assert.IsEmpty(components);
        }

        [Test(Description = "Should throw NotFoundException when learning space id is invalid")]
        public void ListComponents_InvalidLearningSpace_ThrowsNotFoundException()
        {
            Assert.Throws<NotFoundException>(() =>
            {
                var learningSpace = new LearningSpace(invalidId);
                learningSpace.ListComponents();
            });
        }
    }
}
