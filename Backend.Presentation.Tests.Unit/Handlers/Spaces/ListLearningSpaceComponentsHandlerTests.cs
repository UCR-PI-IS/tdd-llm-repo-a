using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers.Spaces
{
    [TestFixture]
    public class ListLearningSpaceComponentsHandlerTests
    {
        [Test]
        public void GetComponents_ShouldReturn200WithComponents_WhenValidIdHasComponents()
        {
            var controller = new LearningSpaceComponentsController();
            var validLearningSpaceId = Guid.NewGuid();
            var response = controller.GetComponents(validLearningSpaceId);
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsNotEmpty(response.Value);
        }

        [Test]
        public void GetComponents_ShouldReturn200EmptyList_WhenValidIdHasNoComponents()
        {
            var controller = new LearningSpaceComponentsController();
            var learningSpaceIdWithNoComponents = Guid.NewGuid();
            var response = controller.GetComponents(learningSpaceIdWithNoComponents);
            Assert.AreEqual(200, response.StatusCode);
            Assert.IsEmpty(response.Value);
        }

        [Test]
        public void GetComponents_ShouldReturn404_WhenInvalidLearningSpaceId()
        {
            var controller = new LearningSpaceComponentsController();
            var invalidLearningSpaceId = Guid.NewGuid();
            var response = controller.GetComponents(invalidLearningSpaceId);
            Assert.AreEqual(404, response.StatusCode);
            Assert.IsInstanceOf<ErrorResponse>(response.Value);
        }
    }

    // Dummy controller for compilation
    public class LearningSpaceComponentsController
    {
        public (int StatusCode, object Value) GetComponents(Guid learningSpaceId)
        {
            if (learningSpaceId == Guid.Empty)
                return (404, new ErrorResponse("Learning space not found"));
            if (learningSpaceId == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                return (200, new List<LearningComponent> { new Whiteboard(), new Projector() });
            if (learningSpaceId == Guid.Parse("00000000-0000-0000-0000-000000000002"))
                return (200, new List<LearningComponent>());
            return (200, new List<LearningComponent>()); // default empty
        }
    }

    public class ErrorResponse
    {
        public string Message { get; }
        public ErrorResponse(string message) => Message = message;
    }

    public class LearningComponent { }
    public class Whiteboard : LearningComponent { }
    public class Projector : LearningComponent { }
}
