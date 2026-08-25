using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningSpace"/> entity constructor and validation.
/// Covers intents Domain-001 through Domain-012.
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
    /// Domain-001: Verify that a LearningSpace entity can be created with all valid
    /// parameters including auto-generated internal ID.
    /// </summary>
    [Test]
    [Description("Domain-001: Verify that a LearningSpace entity can be created with all valid parameters including auto-generated internal ID")]
    public void Constructor_ValidClassroomParameters_AllPropertiesSetCorrectly()
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
            Assert.That(learningSpace.LearningSpaceId, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Domain-009 and Domain-010: Verify that a LearningSpace can be created with each
    /// valid type (Auditorium and Laboratory) and the type is correctly assigned.
    /// </summary>
    [TestCase("Auditorium", 5.0f, 15.0f, 20.0f,
        Description = "Domain-009: Valid type Auditorium")]
    [TestCase("Laboratory", 3.5f, 12.0f, 15.0f,
        Description = "Domain-010: Valid type Laboratory")]
    public void Constructor_ValidType_Succeeds(string type, float height, float width, float length)
    {
        // Arrange & Act
        var learningSpace = new LearningSpace(type, height, width, length);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(learningSpace.Type, Is.EqualTo(type));
            Assert.That(learningSpace.LearningSpaceId, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// Domain-002: Verify that creating a LearningSpace with an invalid type
    /// throws ArgumentException with the expected message.
    /// </summary>
    [Test]
    [Description("Domain-002: Verify that creating a LearningSpace with an invalid type throws ArgumentException")]
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
    /// Domain-003, Domain-004, Domain-005: Verify that creating a LearningSpace with a
    /// zero dimension throws ArgumentException with the expected message.
    /// </summary>
    [TestCase(0.0f, ValidWidth, ValidLength, "Height must be positive and non-zero",
        Description = "Domain-003: Zero height throws ArgumentException")]
    [TestCase(ValidHeight, 0.0f, ValidLength, "Width must be positive and non-zero",
        Description = "Domain-004: Zero width throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, 0.0f, "Length must be positive and non-zero",
        Description = "Domain-005: Zero length throws ArgumentException")]
    public void Constructor_ZeroDimension_ThrowsArgumentException(
        float height, float width, float length, string expectedMessage)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidType, height, width, length);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedMessage));
        });
    }

    /// <summary>
    /// Domain-006, Domain-007, Domain-008: Verify that creating a LearningSpace with a
    /// negative dimension throws ArgumentException with the expected message.
    /// </summary>
    [TestCase(-3.0f, ValidWidth, ValidLength, "Height must be positive and non-zero",
        Description = "Domain-006: Negative height throws ArgumentException")]
    [TestCase(ValidHeight, -8.0f, ValidLength, "Width must be positive and non-zero",
        Description = "Domain-007: Negative width throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, -10.0f, "Length must be positive and non-zero",
        Description = "Domain-008: Negative length throws ArgumentException")]
    public void Constructor_NegativeDimension_ThrowsArgumentException(
        float height, float width, float length, string expectedMessage)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidType, height, width, length);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedMessage));
        });
    }

    /// <summary>
    /// Domain-011: Verify that creating a LearningSpace with a null type
    /// throws ArgumentException with the expected message.
    /// </summary>
    [Test]
    [Description("Domain-011: Verify that creating a LearningSpace with null type throws ArgumentException")]
    public void Constructor_NullType_ThrowsArgumentException()
    {
        // Arrange
        string? type = null;

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(type!, ValidHeight, ValidWidth, ValidLength);
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

    /// <summary>
    /// Domain-012: Verify that creating a LearningSpace with an empty type
    /// throws ArgumentException with the expected message.
    /// </summary>
    [Test]
    [Description("Domain-012: Verify that creating a LearningSpace with empty type throws ArgumentException")]
    public void Constructor_EmptyType_ThrowsArgumentException()
    {
        // Arrange
        var type = "";

        // Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(type, ValidHeight, ValidWidth, ValidLength);
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
