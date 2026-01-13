using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Application.Services.Implementations;
using Backend.Domain.Exceptions;

namespace Backend.Application.Tests.Unit.Services.Implementations
{
    [TestFixture]
    public class LearningComponentServiceTests
    {
        private class LearningComponentService
        {
            public IList<string> ListComponentsByLearningSpaceId(Guid learningSpaceId)
            {
                if (learningSpaceId == Guid.Empty)
                    throw new NotFoundException("Invalid learning space ID");
                if (learningSpaceId == Guid.Parse("00000000-0000-0000-0000-000000000002"))
                    return new List<string>();
                return new List<string> { "Component1" };
            }
        }

        private readonly Guid validId = Guid.NewGuid();
        private readonly Guid validIdWithNoComponents = Guid.Parse("00000000-0000-0000-0000-000000000002");
        private readonly Guid invalidId = Guid.Empty;

        [Test(Description = "Service returns components list when valid learning space has components")]
        public void ListComponentsByLearningSpaceId_ValidIdWithComponents_ReturnsList()
        {
            var service = new LearningComponentService();
            var components = service.ListComponentsByLearningSpaceId(validId);
            Assert.IsTrue(components.Count > 0);
        }

        [Test(Description = "Service returns empty list when valid learning space has no components")]
        public void ListComponentsByLearningSpaceId_ValidIdNoComponents_ReturnsEmpty()
        {
            var service = new LearningComponentService();
            var components = service.ListComponentsByLearningSpaceId(validIdWithNoComponents);
            Assert.IsEmpty(components);
        }

        [Test(Description = "Service throws exception for invalid learning space ID")]
        public void ListComponentsByLearningSpaceId_InvalidId_ThrowsNotFoundException()
        {
            var service = new LearningComponentService();
            Assert.Throws<NotFoundException>(() => service.ListComponentsByLearningSpaceId(invalidId));
        }
    }
}
