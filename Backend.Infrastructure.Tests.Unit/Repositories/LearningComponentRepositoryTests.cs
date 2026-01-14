using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories
{
    [TestFixture]
    public class LearningComponentRepositoryTests
    {
        [Test]
        public void GetComponentsByLearningSpaceId_ShouldReturnComponents_WhenComponentsExist()
        {
            var repository = new LearningComponentRepository();
            var validLearningSpaceId = Guid.NewGuid();
            var components = repository.GetComponentsByLearningSpaceId(validLearningSpaceId);
            Assert.IsNotEmpty(components);
        }

        [Test]
        public void GetComponentsByLearningSpaceId_ShouldReturnEmptyList_WhenNoComponents()
        {
            var repository = new LearningComponentRepository();
            var learningSpaceIdWithNoComponents = Guid.NewGuid();
            var components = repository.GetComponentsByLearningSpaceId(learningSpaceIdWithNoComponents);
            Assert.IsEmpty(components);
        }

        [Test]
        public void GetComponentsByLearningSpaceId_ShouldThrowException_WhenInvalidLearningSpaceId()
        {
            var repository = new LearningComponentRepository();
            var invalidLearningSpaceId = Guid.NewGuid();
            Assert.Throws<Exception>(() => repository.GetComponentsByLearningSpaceId(invalidLearningSpaceId));
        }
    }

    // Dummy repository for compilation
    public class LearningComponentRepository
    {
        public List<LearningComponent> GetComponentsByLearningSpaceId(Guid learningSpaceId)
        {
            if (learningSpaceId == Guid.Empty)
                throw new Exception("Invalid learning space ID");
            // Dummy data for tests
            return new List<LearningComponent> { new Whiteboard(), new Projector() };
        }
    }

    public class LearningComponent { }
    public class Whiteboard : LearningComponent { }
    public class Projector : LearningComponent { }
}
