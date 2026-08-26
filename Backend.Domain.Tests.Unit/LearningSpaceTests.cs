using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the LearningSpace domain entity.
/// </summary>
[TestFixture]
public class LearningSpaceTests
{
    /// <summary>
    /// Verifies that a LearningSpace entity can be created with all valid parameters.
    /// </summary>
    [Test]
    [Description("Creates a learning space with valid Classroom type and dimensions")]
    public void Constructor_WithValidClassroomParameters_CreatesLearningSpace()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act
        var learningSpace = new LearningSpace(id, type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.id, Is.EqualTo(id));
            Assert.That(learningSpace.type, Is.EqualTo(type));
            Assert.That(learningSpace.height, Is.EqualTo(height));
            Assert.That(learningSpace.width, Is.EqualTo(width));
            Assert.That(learningSpace.length, Is.EqualTo(length));
        });
    }

    /// <summary>
    /// Verifies that a LearningSpace can be created with valid type "Auditorium".
    /// </summary>
    [Test]
    [Description("Creates a learning space with valid Auditorium type")]
    public void Constructor_WithValidAuditoriumType_CreatesLearningSpace()
    {
        // Arrange
        var id = "IF-0201";
        var type = "Auditorium";
        var height = 5.0f;
        var width = 15.0f;
        var length = 20.0f;

        // Act
        var learningSpace = new LearningSpace(id, type, height, width, length);

        // Assert
        Assert.That(learningSpace.type, Is.EqualTo(type));
    }

    /// <summary>
    /// Verifies that a LearningSpace can be created with valid type "Laboratory".
    /// </summary>
    [Test]
    [Description("Creates a learning space with valid Laboratory type")]
    public void Constructor_WithValidLaboratoryType_CreatesLearningSpace()
    {
        // Arrange
        var id = "IF-0301";
        var type = "Laboratory";
        var height = 3.5f;
        var width = 12.0f;
        var length = 15.0f;

        // Act
        var learningSpace = new LearningSpace(id, type, height, width, length);

        // Assert
        Assert.That(learningSpace.type, Is.EqualTo(type));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that zero height throws ArgumentException")]
    public void Constructor_WithZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 0.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that zero width throws ArgumentException")]
    public void Constructor_WithZeroWidth_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 0.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with zero length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that zero length throws ArgumentException")]
    public void Constructor_WithZeroLength_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that negative height throws ArgumentException")]
    public void Constructor_WithNegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = -3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that negative width throws ArgumentException")]
    public void Constructor_WithNegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = -8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with negative length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that negative length throws ArgumentException")]
    public void Constructor_WithNegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = -10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with null id throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that null id throws ArgumentException")]
    public void Constructor_WithNullId_ThrowsArgumentException()
    {
        // Arrange
        string? id = null;
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id!, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Id is required"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with empty id throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that empty id throws ArgumentException")]
    public void Constructor_WithEmptyId_ThrowsArgumentException()
    {
        // Arrange
        var id = "";
        var type = "Classroom";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Id is required"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with null type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that null type throws ArgumentException")]
    public void Constructor_WithNullType_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        string? type = null;
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type!, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with empty type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that empty type throws ArgumentException")]
    public void Constructor_WithEmptyType_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var type = "";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }

    /// <summary>
    /// Verifies that creating a LearningSpace with an invalid type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Validates that invalid type throws ArgumentException")]
    public void Constructor_WithInvalidType_ThrowsArgumentException()
    {
        // Arrange
        var id = "IF-0101";
        var invalidType = "InvalidType";
        var height = 3.0f;
        var width = 8.0f;
        var length = 10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(id, invalidType, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
    }
}
