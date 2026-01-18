using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit
{
    [TestFixture]
    public class LearningSpaceApplicationTests
    {
        private class FakeLearningSpaceRepository : ILearningSpaceListRepository
        {
            private Dictionary<int, List<LearningComponent>> data = new();

            public void SetComponentsForLearningSpace(int id, List<LearningComponent> components)
            {
                data[id] = components;
            }

            public void SetInvalidLearningSpace(int id)
            {
                data.Remove(id);
            }

            public List<LearningComponent> GetComponentsByLearningSpaceId(int learningSpaceId)
            {
                if (!data.ContainsKey(learningSpaceId))
                {
                    throw new InvalidLearningSpaceException($"Learning space ID {learningSpaceId} is invalid.");
                }
                return data[learningSpaceId];
            }
        }

        private LearningSpaceListService service;
        private FakeLearningSpaceRepository fakeRepo;

        [SetUp]
        public void Setup()
        {
            fakeRepo = new FakeLearningSpaceRepository();
            service = new LearningSpaceListService(fakeRepo);
        }

        [Test(Description = "Returns correct components when repository returns them")]
        public void ListComponents_ValidLearningSpace_ReturnsComponents()
        {
            fakeRepo.SetComponentsForLearningSpace(1, new List<LearningComponent>
            {
                new LearningComponent("Whiteboard"),
                new LearningComponent("Projector"),
            });

            var components = service.ListComponents(1);

            Assert.AreEqual(2, components.Count);
            Assert.IsTrue(components.Any(c => c.Name == "Whiteboard"));
        }

        [Test(Description = "Returns empty list when repository returns empty list")]
        public void ListComponents_ValidLearningSpaceNoComponents_ReturnsEmptyList()
        {
            fakeRepo.SetComponentsForLearningSpace(2, new List<LearningComponent>());
            var components = service.ListComponents(2);
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test(Description = "Throws exception when invalid learning space id requested")]
        public void ListComponents_InvalidLearningSpace_ThrowsInvalidLearningSpaceException()
        {
            fakeRepo.SetInvalidLearningSpace(999);
            Assert.Throws<InvalidLearningSpaceException>(() =>
            {
                service.ListComponents(999);
            });
        }
    }
}
