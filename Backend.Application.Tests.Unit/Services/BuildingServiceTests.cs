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
    public class BuildingServiceTests
    {
        private Mock<IBuildingRepository> _mockRepository;
        private BuildingService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<IBuildingRepository>();
            _service = new BuildingService(_mockRepository.Object);
        }

        [Test(Description = "Successfully adds building when name does not exist")]
        public async Task AddBuildingAsync_WithUniqueName_ReturnsTrue()
        {
            // Arrange
            string name = "Engineering Building";
            string color = "Blue";
            double height = 15.5;
            double length = 30.0;
            double width = 20.0;
            double x = 100.0;
            double y = 200.0;
            double z = 0.0;

            _mockRepository.Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync((Building)null);
            
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Building>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.AddBuildingAsync(name, color, height, length, width, x, y, z);

            // Assert
            Assert.That(result, Is.True);
            _mockRepository.Verify(r => r.GetByNameAsync(name), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.Is<Building>(b => 
                b.Name == name && 
                b.Color == color && 
                b.Height == height &&
                b.Length == length &&
                b.Width == width &&
                b.X == x &&
                b.Y == y &&
                b.Z == z)), Times.Once);
        }

        [Test(Description = "Throws InvalidOperationException when building name already exists")]
        public void AddBuildingAsync_WithDuplicateName_ThrowsInvalidOperationException()
        {
            // Arrange
            string name = "Engineering Building";
            var existingBuilding = new Building(1, name, "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0);

            _mockRepository.Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync(existingBuilding);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _service.AddBuildingAsync(name, "Red", 20.0, 40.0, 25.0, 150.0, 250.0, 5.0));

            Assert.That(ex.Message, Does.Contain("Building with name"));
            Assert.That(ex.Message, Does.Contain("already exists"));
            _mockRepository.Verify(r => r.GetByNameAsync(name), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
        }

        [Test]
        [TestCase(null, Description = "Null name throws ArgumentNullException")]
        public void AddBuildingAsync_WithNullName_ThrowsArgumentNullException(string name)
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.AddBuildingAsync(name, "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
            _mockRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
        }

        [Test]
        [TestCase(null, Description = "Null color throws ArgumentNullException")]
        public void AddBuildingAsync_WithNullColor_ThrowsArgumentNullException(string color)
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _service.AddBuildingAsync("Engineering Building", color, 15.5, 30.0, 20.0, 100.0, 200.0, 0.0));

            Assert.That(ex.ParamName, Is.EqualTo("color"));
            _mockRepository.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Building>()), Times.Never);
        }

        [Test(Description = "Repository is called with correct building data")]
        public async Task AddBuildingAsync_CallsRepositoryWithCorrectData()
        {
            // Arrange
            string name = "Science Building";
            string color = "Green";
            double height = 20.0;
            double length = 40.0;
            double width = 30.0;
            double x = 150.0;
            double y = 250.0;
            double z = 5.0;

            _mockRepository.Setup(r => r.GetByNameAsync(name))
                .ReturnsAsync((Building)null);
            
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Building>()))
                .ReturnsAsync(true);

            // Act
            await _service.AddBuildingAsync(name, color, height, length, width, x, y, z);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.Is<Building>(b =>
                b.Name == name &&
                b.Color == color &&
                b.Height == height &&
                b.Length == length &&
                b.Width == width &&
                b.X == x &&
                b.Y == y &&
                b.Z == z)), Times.Once);
        }
    }
}
