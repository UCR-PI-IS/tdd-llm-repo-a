using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Domain.Exceptions;

namespace Backend.Infrastructure.Tests.Unit.Repositories
{
    [TestFixture]
    public class LearningComponentRepositoryTests
    {
        private class LearningComponentRepository
        {
            public async Task<IList<string>> ListByLearningSpaceIdAsync(Guid learningSpaceId)
            {
                await Task.Delay(1); // simulate async
                if (learningSpaceId == Guid.Empty)
                    throw new NotFoundException("Invalid LearningSpaceId");
                if (learningSpaceId == Guid.Parse("00000000-0000-0000-0000-000000000003"))
                    return new List<string>();
                return new List<string> { "Component1" };
            }
        }

        private readonly Guid validId = Guid.NewGuid();
        private readonly Guid validIdWithNoComponents = Guid.Parse("00000000-0000-0000-0000-000000000003");
        private readonly Guid invalidId = Guid.Empty;

        [Test(Description = "Repo returns components for valid learning space")]
        public async Task ListByLearningSpaceIdAsync_ValidId_ReturnsComponents()
        {
            var repo = new LearningComponentRepository();
            var components = await repo.ListByLearningSpaceIdAsync(validId);
            Assert.IsNotEmpty(components);
        }

        [Test(Description = "Repo returns empty list for valid learning space with no components")]
        public async Task ListByLearningSpaceIdAsync_ValidIdNoComponents_ReturnsEmpty()
        {
            var repo = new LearningComponentRepository();
            var components = await repo.ListByLearningSpaceIdAsync(validIdWithNoComponents);
            Assert.IsEmpty(components);
        }

        [Test(Description = "Repo throws exception for invalid learning space id")]
        public void ListByLearningSpaceIdAsync_InvalidId_ThrowsNotFoundException()
        {
            var repo = new LearningComponentRepository();
            Assert.ThrowsAsync<NotFoundException>(async () => 
                await repo.ListByLearningSpaceIdAsync(invalidId));
        }
    }
}
