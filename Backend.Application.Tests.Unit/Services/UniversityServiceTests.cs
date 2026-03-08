using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit.Services
{
    [TestFixture]
    public class UniversityServiceTests
    {
        private Mock<IUniversityRepository> _mockRepository;
        private UniversityService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IUniversityRepository>();
            _service = new UniversityService(_mockRepository.Object);
        }

        [Test]
        [Description("AddUniversityAsync returns true when university does not exist")]
        public async Task AddUniversityAsync_UniversityDoesNotExist_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByNameAsync("UCR")).ReturnsAsync((University)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<University>())).ReturnsAsync(true);

            // Act
            var result = await _service.AddUniversityAsync("UCR", "Costa Rica");

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.GetByNameAsync("UCR"), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.Is<University>(u => 
                u.Name == "UCR" && u.Country == "Costa Rica")), Times.Once);
        }

        [Test]
        [Description("AddUniversityAsync throws InvalidOperationException when university name already exists")]
        public void AddUniversityAsync_UniversityAlreadyExists_ThrowsInvalidOperationException()
        {
            // Arrange
            var existingUniversity = new University(1, "UCR", "Costa Rica");
            _mockRepository.Setup(r => r.GetByNameAsync("UCR")).ReturnsAsync(existingUniversity);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddUniversityAsync("UCR", "Panama"));

            Assert.That(ex.Message, Does.Contain("University with name"));
            Assert.That(ex.Message, Does.Contain("already exists"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        }

        [Test]
        [Description("AddUniversityAsync returns false when repository AddAsync fails")]
        public async Task AddUniversityAsync_RepositoryAddFails_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetByNameAsync("UCR")).ReturnsAsync((University)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<University>())).ReturnsAsync(false);

            // Act
            var result = await _service.AddUniversityAsync("UCR", "Costa Rica");

            // Assert
            Assert.That(result, Is.False);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Once);
        }
    }
}
