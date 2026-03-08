using NUnit.Framework;
using System;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities
{
    [TestFixture]
    public class WhiteboardTests
    {
        [Test]
        [TestCase("WB-001", 2.0, 1.5, Description = "Creates whiteboard with valid positive dimensions")]
        [TestCase("WB-002", 0.1, 0.1, Description = "Creates whiteboard with minimum positive dimensions")]
        [TestCase("WB-003", 10.0, 5.0, Description = "Creates whiteboard with large dimensions")]
        public void Constructor_WithValidParameters_CreatesWhiteboardSuccessfully(string id, double width, double height)
        {
            // Act
            var whiteboard = new Whiteboard(id, width, height);

            // Assert
            Assert.That(whiteboard.Id, Is.EqualTo(id));
            Assert.That(whiteboard.Width, Is.EqualTo(width));
            Assert.That(whiteboard.Height, Is.EqualTo(height));
        }

        [Test]
        [TestCase("WB-001", -1.0, 1.5, Description = "Negative width throws ArgumentException")]
        [TestCase("WB-002", -0.1, 2.0, Description = "Small negative width throws ArgumentException")]
        [TestCase("WB-003", -10.0, 1.0, Description = "Large negative width throws ArgumentException")]
        public void Constructor_WithNegativeWidth_ThrowsArgumentException(string id, double width, double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Whiteboard(id, width, height));
            Assert.That(ex.Message, Does.Contain("width").IgnoreCase);
        }

        [Test]
        [TestCase("WB-001", 2.0, -1.5, Description = "Negative height throws ArgumentException")]
        [TestCase("WB-002", 1.0, -0.1, Description = "Small negative height throws ArgumentException")]
        [TestCase("WB-003", 3.0, -10.0, Description = "Large negative height throws ArgumentException")]
        public void Constructor_WithNegativeHeight_ThrowsArgumentException(string id, double width, double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Whiteboard(id, width, height));
            Assert.That(ex.Message, Does.Contain("height").IgnoreCase);
        }

        [Test]
        [TestCase("WB-001", 0.0, 1.5, Description = "Zero width throws ArgumentException")]
        public void Constructor_WithZeroWidth_ThrowsArgumentException(string id, double width, double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Whiteboard(id, width, height));
            Assert.That(ex.Message, Does.Contain("width").IgnoreCase);
        }

        [Test]
        [TestCase("WB-001", 2.0, 0.0, Description = "Zero height throws ArgumentException")]
        public void Constructor_WithZeroHeight_ThrowsArgumentException(string id, double width, double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Whiteboard(id, width, height));
            Assert.That(ex.Message, Does.Contain("height").IgnoreCase);
        }

        [Test]
        [TestCase(null, 2.0, 1.5, Description = "Null ID throws ArgumentException")]
        [TestCase("", 2.0, 1.5, Description = "Empty ID throws ArgumentException")]
        [TestCase("   ", 2.0, 1.5, Description = "Whitespace ID throws ArgumentException")]
        public void Constructor_WithInvalidId_ThrowsArgumentException(string id, double width, double height)
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => new Whiteboard(id, width, height));
            Assert.That(ex.Message, Does.Contain("id").IgnoreCase);
        }
    }

    [TestFixture]
    public class LearningSpaceWhiteboardTests
    {
        [Test]
        [TestCase(5.0, 5.0, 2.0, 1.5, true, Description = "Whiteboard fits when smaller than learning space")]
        [TestCase(5.0, 5.0, 5.0, 5.0, true, Description = "Whiteboard fits when equal to learning space dimensions")]
        [TestCase(5.0, 5.0, 4.9, 4.9, true, Description = "Whiteboard fits when slightly smaller than learning space")]
        public void CanFitWhiteboard_WhenWhiteboardFits_ReturnsTrue(double spaceWidth, double spaceHeight, double whiteboardWidth, double whiteboardHeight)
        {
            // Arrange
            var learningSpace = new LearningSpace("LS-001", "Test Space", spaceWidth, spaceHeight);
            var whiteboard = new Whiteboard("WB-001", whiteboardWidth, whiteboardHeight);

            // Act
            var result = learningSpace.CanFitWhiteboard(whiteboard);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        [TestCase(5.0, 5.0, 5.1, 1.5, false, Description = "Whiteboard doesn't fit when width exceeds space")]
        [TestCase(5.0, 5.0, 2.0, 5.1, false, Description = "Whiteboard doesn't fit when height exceeds space")]
        [TestCase(5.0, 5.0, 6.0, 6.0, false, Description = "Whiteboard doesn't fit when both dimensions exceed space")]
        [TestCase(5.0, 5.0, 5.1, 5.1, false, Description = "Whiteboard doesn't fit when slightly larger than space")]
        public void CanFitWhiteboard_WhenWhiteboardDoesNotFit_ReturnsFalse(double spaceWidth, double spaceHeight, double whiteboardWidth, double whiteboardHeight)
        {
            // Arrange
            var learningSpace = new LearningSpace("LS-001", "Test Space", spaceWidth, spaceHeight);
            var whiteboard = new Whiteboard("WB-001", whiteboardWidth, whiteboardHeight);

            // Act
            var result = learningSpace.CanFitWhiteboard(whiteboard);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
