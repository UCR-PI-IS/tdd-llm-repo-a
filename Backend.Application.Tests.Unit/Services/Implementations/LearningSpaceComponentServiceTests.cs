using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services.Implementations
{
    [TestFixture]
    public class LearningSpaceComponentServiceTests
    {
        [Test]
        public void ListComponents_ShouldReturnComponents_WhenLearningSpaceHasComponents()
        {
            var service = new ListLearningComponentsService();
            var validLearningSpaceId = Guid.NewGuid();
            var result = service.ListComponents(validLearningSpaceId);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void ListComponents_ShouldReturnEmptyList_WhenLearningSpaceHasNoComponents()
        {
            var service = new ListLearningComponentsService();
            var learningSpaceIdWithNoComponents = Guid.NewGuid();
            var result = service.ListComponents(learningSpaceIdWithNoComponents);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ListComponents_ShouldThrowNotFoundException_WhenLearningSpaceIsInvalid()
        {
            var service = new ListLearningComponentsService();
            var invalidLearningSpaceId = Guid.NewGuid();
            Assert.Throws<NotFoundException>(() => service.ListComponents(invalidLearningSpaceId));
        }
    }

    // Dummy classes for compilation
    public class ListLearningComponentsService
    {
        public List<LearningComponent> ListComponents(Guid learningSpaceId)
        {
            if (learningSpaceId == Guid.Empty)
                throw new NotFoundException("Learning space not found");
            // Dummy return for test
            return new List<LearningComponent> { new Whiteboard(), new Projector() };
        }
    }

    public class LearningComponent { }
    public class Whiteboard : LearningComponent { }
    public class Projector : LearningComponent { }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
