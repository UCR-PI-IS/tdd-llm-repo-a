using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningSpace"/> entity.
/// </summary>
[TestFixture]
public class LearningSpaceTests
{
    /// <summary>
    /// Verifies that a LearningSpace entity can be created with all valid parameters including auto-generated internal ID.
    /// </summary>
    [Test]
    [Description("Create a new learning space with all specifications and internal ID")]
    public void Constructor_ValidClassroomParameters_CreatesLearningSpace()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act
        var learningSpace = new LearningSpace(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.Type, Is.EqualTo(type));
            Assert.That(learningSpace.Height, Is.EqualTo(height));
            Assert.That(learningSpace.Width, Is.EqualTo(width));
            Assert.That(learningSpace.Length, Is.EqualTo(length));
            Assert.That(learningSpace.LearningSpaceId, Is.GreaterThan(0));
        });
    }

    /// <summary>
    /// Verifies that a LearningSpace can be created with valid type "Auditorium".
    /// </summary>
    [Test]
    [Description("Create a new learning space with all specifications and internal ID")]
    public void Constructor_ValidAuditoriumParameters_CreatesLearningSpace()
    {
        // Arrange
        var type = "Auditorium";
        var height = 5.0f;
        var width = 15.0f;
        var length = 20.0f;

        // Act
        var learningSpace = new LearningSpace(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.Type, Is.EqualTo(type));
            Assert.That(learningSpace.LearningSpaceId, Is.GreaterThan(0));
        });
    }

    /// <summary>
    /// Verifies that a LearningSpace can be created with valid type "Laboratory".
    /// </summary>
    [Test]
    [Description("Create a new learning space with all specifications and internal ID")]
    public void Constructor_ValidLaboratoryParameters_CreatesLearningSpace()
    {
        // Arrange
        var type = "Laboratory";
        var height = 3.5f;
        var width = 12.0f;
        var length = 15.0f;

        // Act
        var learningSpace = new LearningSpace(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.Type, Is.EqualTo(type));
            Assert.That(learningSpace.LearningSpaceId, Is.GreaterThan(0));
        });
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with an invalid type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(invalidType, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_ZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 0.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_ZeroWidth_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 0.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_ZeroLength_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = -3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = -8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_NegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = -10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with null type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_NullType_ThrowsArgumentException()
    {
        // Arrange
        string? type = null;
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type!, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with empty type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Missing required fields when creating a learning space")]
    public void Constructor_EmptyType_ThrowsArgumentException()
    {
        // Arrange
        var type = "";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }
}
