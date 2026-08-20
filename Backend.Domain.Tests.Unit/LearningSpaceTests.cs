using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningSpace"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-012 for story SQL-LS-001-007.
/// </summary>
[TestFixture]
public class LearningSpaceTests
{
    // Valid test data constants
    private const string ValidType = "Classroom";
    private const float ValidHeight = 3.0f;
    private const float ValidWidth = 8.0f;
    private const float ValidLength = 10.0f;

    /// <summary>
    /// Domain-001: Verify that a LearningSpace entity can be created with all valid parameters
    /// including auto-generated internal ID.
    /// </summary>
    [Test]
    [Description("Domain-001: Create LearningSpace with valid parameters and verify all properties including auto-generated ID")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var type = ValidType;
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
    [Description("Domain-002: Invalid type throws ArgumentException with descriptive message")]
    public void Constructor_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(invalidType, ValidHeight, ValidWidth, ValidLength);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
        });
    }

    /// <summary>
    /// Domain-003 and Domain-006: Verify that creating a LearningSpace with zero or negative height
    /// throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-003: Zero height throws ArgumentException")]
    [TestCase(-3.0f, Description = "Domain-006: Negative height throws ArgumentException")]
    public void Constructor_InvalidHeight_ThrowsArgumentException(float invalidHeight)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidType, invalidHeight, ValidWidth, ValidLength);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Height must be positive and non-zero"));
        });
    }

    /// <summary>
    /// Domain-004 and Domain-007: Verify that creating a LearningSpace with zero or negative width
    /// throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-004: Zero width throws ArgumentException")]
    [TestCase(-8.0f, Description = "Domain-007: Negative width throws ArgumentException")]
    public void Constructor_InvalidWidth_ThrowsArgumentException(float invalidWidth)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidType, ValidHeight, invalidWidth, ValidLength);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Width must be positive and non-zero"));
        });
    }

    /// <summary>
    /// Domain-005 and Domain-008: Verify that creating a LearningSpace with zero or negative length
    /// throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(-10.0f, Description = "Domain-008: Negative length throws ArgumentException")]
    public void Constructor_InvalidLength_ThrowsArgumentException(float invalidLength)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidType, ValidHeight, ValidWidth, invalidLength);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Length must be positive and non-zero"));
        });
    }

    /// <summary>
    /// Domain-009 and Domain-010: Verify that a LearningSpace can be created with valid types
    /// "Auditorium" and "Laboratory".
    /// </summary>
    [TestCase("Auditorium", 5.0f, 15.0f, 20.0f,
        Description = "Domain-009: Valid type Auditorium succeeds")]
    [TestCase("Laboratory", 3.5f, 12.0f, 15.0f,
        Description = "Domain-010: Valid type Laboratory succeeds")]
    public void Constructor_ValidType_Succeeds(string type, float height, float width, float length)
    {
        // Arrange & Act
        var learningSpace = new LearningSpace(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.Type, Is.EqualTo(type));
            Assert.That(learningSpace.LearningSpaceId, Is.GreaterThan(0));
        });
    }

    /// <summary>
    /// Domain-011 and Domain-012: Verify that creating a LearningSpace with null or empty type
    /// throws ArgumentException.
    /// </summary>
    [TestCase(null, Description = "Domain-011: Null type throws ArgumentException")]
    [TestCase("", Description = "Domain-012: Empty type throws ArgumentException")]
    public void Constructor_NullOrEmptyType_ThrowsArgumentException(string? invalidType)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(invalidType!, ValidHeight, ValidWidth, ValidLength);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain("Type is required"));
        });
    }
}
