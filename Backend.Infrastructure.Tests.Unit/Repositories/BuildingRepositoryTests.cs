using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories
{
    [TestFixture]
    public class BuildingRepositoryTests
    {
        private UCRDatabaseContext _context;
        private BuildingRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{System.Guid.NewGuid()}")
                .Options;

            _context = new UCRDatabaseContext(options);
            _repository = new BuildingRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test(Description = "Successfully adds building to database")]
        public async Task AddAsync_WithValidBuilding_ReturnsTrue()
        {
            // Arrange
            var building = new Building(1, "Engineering Building", "Blue", 15.5, 30.0, 20.0, 100.0, 200.0, 0.0);

            // Act
            var result = await _repository.AddAsync(building);

            // Assert
            Assert.That(result, Is.True);
            
            var savedBuilding = await _repository.GetByNameAsync("Engineering Building");
            Assert.That(savedBuilding, Is.Not.Null);
            Assert.That(savedBuilding.Name, Is.EqualTo("Engineering Building"));
            Assert.That(savedBuilding.Color, Is.EqualTo("Blue"));
            Assert.That(savedBuilding.Height, Is.EqualTo(15.5));
            Assert.That(savedBuilding.Length, Is.EqualTo(30.0));
            Assert.That(savedBuilding.Width, Is.EqualTo(20.0));
            Assert.That(savedBuilding.X, Is.EqualTo(100.0));
            Assert.That(savedBuilding.Y, Is.EqualTo(200.0));
            Assert.That(savedBuilding.Z, Is.EqualTo(0.0));
        }

        [Test(Description = "GetByNameAsync returns null when building not found")]
        public async Task GetByNameAsync_WithNonExistentName_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByNameAsync("NonExistentBuilding");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test(Description = "GetByNameAsync returns existing building")]
        public async Task GetByNameAsync_WithExistingName_ReturnsBuilding()
        {
            // Arrange
            var building = new Building(1, "Science Building", "Green", 20.0, 40.0, 30.0, 150.0, 250.0, 0.0);
            await _repository.AddAsync(building);

            // Act
            var result = await _repository.GetByNameAsync("Science Building");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Science Building"));
            Assert.That(result.Color, Is.EqualTo("Green"));
            Assert.That(result.Height, Is.EqualTo(20.0));
            Assert.That(result.Length, Is.EqualTo(40.0));
            Assert.That(result.Width, Is.EqualTo(30.0));
            Assert.That(result.X, Is.EqualTo(150.0));
            Assert.That(result.Y, Is.EqualTo(250.0));
            Assert.That(result.Z, Is.EqualTo(0.0));
        }
    }
}
