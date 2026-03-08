using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Tests.Unit.Repositories
{
    [TestFixture]
    public class WhiteboardRepositoryTests
    {
        private UCRDatabaseContext _context;
        private WhiteboardRepository _repository;
        private DbContextOptions<UCRDatabaseContext> _options;

        [SetUp]
        public void SetUp()
        {
            _options = new DbContextOptionsBuilder<UCRDatabaseContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{System.Guid.NewGuid()}")
                .Options;
            _context = new UCRDatabaseContext(_options);
            _repository = new WhiteboardRepository(_context);
        }

        [Test]
        [TestCase("WB-001", 2.0, 1.5, Description = "Successfully adds whiteboard with standard dimensions")]
        [TestCase("WB-002", 5.0, 5.0, Description = "Successfully adds whiteboard with equal dimensions")]
        [TestCase("WB-003", 0.5, 0.5, Description = "Successfully adds whiteboard with small dimensions")]
        public async Task AddAsync_WithValidWhiteboard_AddsWhiteboardToDatabase(string id, double width, double height)
        {
            // Arrange
            var whiteboard = new Whiteboard(id, width, height);

            // Act
            await _repository.AddAsync(whiteboard);
            await _context.SaveChangesAsync();

            // Assert
            var saved = await _context.Whiteboards.FindAsync(id);
            Assert.That(saved, Is.Not.Null);
            Assert.That(saved.Id, Is.EqualTo(id));
            Assert.That(saved.Width, Is.EqualTo(width));
            Assert.That(saved.Height, Is.EqualTo(height));
        }

        [Test]
        [TestCase("WB-001", 2.0, 1.5, Description = "Successfully retrieves whiteboard by ID")]
        [TestCase("WB-002", 5.0, 5.0, Description = "Successfully retrieves whiteboard with different dimensions")]
        [TestCase("WB-003", 10.0, 7.5, Description = "Successfully retrieves whiteboard with large dimensions")]
        public async Task GetByIdAsync_WithExistingWhiteboard_ReturnsWhiteboard(string id, double width, double height)
        {
            // Arrange
            var whiteboard = new Whiteboard(id, width, height);
            await _repository.AddAsync(whiteboard);
            await _context.SaveChangesAsync();

            // Act
            var retrieved = await _repository.GetByIdAsync(id);

            // Assert
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.Id, Is.EqualTo(id));
            Assert.That(retrieved.Width, Is.EqualTo(width));
            Assert.That(retrieved.Height, Is.EqualTo(height));
        }

        [Test]
        [TestCase("WB-999", Description = "Returns null when whiteboard does not exist")]
        [TestCase("WB-INVALID", Description = "Returns null when whiteboard ID is invalid")]
        [TestCase("", Description = "Returns null when ID is empty string")]
        public async Task GetByIdAsync_WithNonExistentWhiteboard_ReturnsNull(string id)
        {
            // Act
            var retrieved = await _repository.GetByIdAsync(id);

            // Assert
            Assert.That(retrieved, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
            _repository = null;
        }
    }
}
