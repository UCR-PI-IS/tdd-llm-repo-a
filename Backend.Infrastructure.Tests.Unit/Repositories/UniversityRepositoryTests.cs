using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories
{
    [TestFixture]
    public class UniversityRepositoryTests
    {
        private UCRDatabaseContext _context;
        private UniversityRepository _repository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<UCRDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"TestDatabase_{System.Guid.NewGuid()}")
                .Options;

            _context = new UCRDatabaseContext(options);
            _repository = new UniversityRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        [Description("GetByNameAsync returns university when found")]
        public async Task GetByNameAsync_UniversityExists_ReturnsUniversity()
        {
            // Arrange
            var university = new University(1, "UCR", "Costa Rica");
            _context.Universities.Add(university);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByNameAsync("UCR");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("UCR"));
            Assert.That(result.Country, Is.EqualTo("Costa Rica"));
        }

        [Test]
        [Description("GetByNameAsync returns null when university not found")]
        public async Task GetByNameAsync_UniversityDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByNameAsync("NonExistent");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        [Description("AddAsync successfully adds university to database")]
        public async Task AddAsync_ValidUniversity_AddsToDatabase()
        {
            // Arrange
            var university = new University(0, "UCR", "Costa Rica");

            // Act
            var result = await _repository.AddAsync(university);

            // Assert
            Assert.That(result, Is.True);
            var saved = await _context.Universities.FirstOrDefaultAsync(u => u.Name == "UCR");
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.Country, Is.EqualTo("Costa Rica"));
        }
    }
}
