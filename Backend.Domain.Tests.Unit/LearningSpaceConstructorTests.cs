using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningSpace"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-012.
/// </summary>
[TestFixture]
public class LearningSpaceConstructorTests
{
    // Valid test data constants
    private const string ValidTypeClassroom = "Classroom";
    private const string ValidTypeAuditorium = "Auditorium";
    private const string ValidTypeLaboratory = "Laboratory";
    private const float ValidHeight = 3.0f;
    private const float ValidWidth = 8.0f;
    private const float ValidLength = 10.0f;

    /// <summary>
    /// Domain-001: Verify that a LearningSpace entity can be created with all valid parameters
    /// including auto-generated internal ID.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningSpace entity can be created with all valid parameters")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = ValidHeight;
        var width = ValidWidth;
        var length = ValidLength;

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
    /// Domain-002: Verify that creating a LearningSpace with an invalid type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify that creating a LearningSpace with invalid type throws ArgumentException")]
    public void Constructor_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";
        var height = ValidHeight;
        var width = ValidWidth;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(invalidType, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
    }

    /// <summary>
    /// Domain-003: Verify that creating a LearningSpace with zero height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-003: Verify that creating a LearningSpace with zero height throws ArgumentException")]
    public void Constructor_ZeroHeight_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = 0.0f;
        var width = ValidWidth;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-004: Verify that creating a LearningSpace with zero width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-004: Verify that creating a LearningSpace with zero width throws ArgumentException")]
    public void Constructor_ZeroWidth_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = ValidHeight;
        var width = 0.0f;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-005: Verify that creating a LearningSpace with zero length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-005: Verify that creating a LearningSpace with zero length throws ArgumentException")]
    public void Constructor_ZeroLength_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = ValidHeight;
        var width = ValidWidth;
        var length = 0.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-006: Verify that creating a LearningSpace with negative height throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-006: Verify that creating a LearningSpace with negative height throws ArgumentException")]
    public void Constructor_NegativeHeight_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = -3.0f;
        var width = ValidWidth;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Height must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-007: Verify that creating a LearningSpace with negative width throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-007: Verify that creating a LearningSpace with negative width throws ArgumentException")]
    public void Constructor_NegativeWidth_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = ValidHeight;
        var width = -8.0f;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Width must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-008: Verify that creating a LearningSpace with negative length throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-008: Verify that creating a LearningSpace with negative length throws ArgumentException")]
    public void Constructor_NegativeLength_ThrowsArgumentException()
    {
        // Arrange
        var type = ValidTypeClassroom;
        var height = ValidHeight;
        var width = ValidWidth;
        var length = -10.0f;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Length must be positive and non-zero"));
    }

    /// <summary>
    /// Domain-009: Verify that a LearningSpace can be created with valid type "Auditorium".
    /// </summary>
    [Test]
    [Description("Domain-009: Verify that a LearningSpace can be created with valid type Auditorium")]
    public void Constructor_ValidTypeAuditorium_Succeeds()
    {
        // Arrange
        var type = ValidTypeAuditorium;
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
    /// Domain-010: Verify that a LearningSpace can be created with valid type "Laboratory".
    /// </summary>
    [Test]
    [Description("Domain-010: Verify that a LearningSpace can be created with valid type Laboratory")]
    public void Constructor_ValidTypeLaboratory_Succeeds()
    {
        // Arrange
        var type = ValidTypeLaboratory;
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
    /// Domain-011: Verify that creating a LearningSpace with null type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-011: Verify that creating a LearningSpace with null type throws ArgumentException")]
    public void Constructor_NullType_ThrowsArgumentException()
    {
        // Arrange
        string? type = null;
        var height = ValidHeight;
        var width = ValidWidth;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type!, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }

    /// <summary>
    /// Domain-012: Verify that creating a LearningSpace with empty type throws ArgumentException.
    /// </summary>
    [Test]
    [Description("Domain-012: Verify that creating a LearningSpace with empty type throws ArgumentException")]
    public void Constructor_EmptyType_ThrowsArgumentException()
    {
        // Arrange
        var type = "";
        var height = ValidHeight;
        var width = ValidWidth;
        var length = ValidLength;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex.Message, Does.Contain("Type is required"));
    }
}
