using NUnit.Framework;
using System;
using System.Collections.Generic;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit
{
    [TestFixture]
    public class LearningSpaceInfrastructureTests
    {
        private SqlLearningSpaceListRepository repository;

        [SetUp]
        public void Setup()
        {
            // Provide a fake dbContext or use a test constructor, so test instantiates
            repository = (SqlLearningSpaceListRepository)Activator.CreateInstance(typeof(SqlLearningSpaceListRepository), true);
        }

        [Test(Description = "Returns list of components for valid learning space ID")]
        public void GetComponentsByLearningSpaceId_ValidId_ReturnsComponents()
        {
            var components = repository.GetComponentsByLearningSpaceId(1);
            Assert.IsNotNull(components);
            Assert.IsTrue(components.Count > 0);
        }

        [Test(Description = "Returns empty list for valid learning space ID with no components")]
        public void GetComponentsByLearningSpaceId_ValidIdNoComponents_ReturnsEmptyList()
        {
            var components = repository.GetComponentsByLearningSpaceId(2);
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test(Description = "Throws exception for invalid learning space ID")]
        public void GetComponentsByLearningSpaceId_InvalidId_ThrowsInvalidLearningSpaceException()
        {
            Assert.Throws<InvalidLearningSpaceException>(() =>
            {
                repository.GetComponentsByLearningSpaceId(-1);
            });
        }
    }
}
