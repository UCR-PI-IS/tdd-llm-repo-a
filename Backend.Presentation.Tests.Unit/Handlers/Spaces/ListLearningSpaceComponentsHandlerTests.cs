using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers.Spaces
{
    [TestFixture]
    public class ListLearningSpaceComponentsHandlerTests
    {
        [Test]
        public void GetComponents_Returns200WithComponents_WhenValidIdHasComponents()
        {
            var controller = new LearningSpaceComponentsController();
            var validLearningSpaceId = Guid.NewGuid();
            var response = controller.GetComponents(validLearningSpaceId);
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsNotEmpty(response.Value);
        }

        [Test]
        public void GetComponents_Returns200EmptyList_WhenValidIdHasNoComponents()
        {
            var controller = new LearningSpaceComponentsController();
            var learningSpaceIdWithNoComponents = Guid.NewGuid();
            var response = controller.GetComponents(learningSpaceIdWithNoComponents);
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsEmpty(response.Value);
        }

        [Test]
        public void GetComponents_Returns404_WhenInvalidLearningSpaceId()
        {
            var controller = new LearningSpaceComponentsController();
            var invalidLearningSpaceId = Guid.NewGuid();
            var response = controller.GetComponents(invalidLearningSpaceId);
            Assert.AreEqual(404, response.StatusCode);
            Assert.IsInstanceOf<ErrorResponse>(response.Value);
        }
    }
}
