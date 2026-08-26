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
    /// Domain-002, Domain-011, Domain-012: Verify that creating a LearningSpace with an invalid,
    /// null, or empty type throws ArgumentException.
    /// </summary>
    [TestCase("InvalidType", Description = "Domain-002: Invalid type throws ArgumentException")]
    [TestCase(null, Description = "Domain-011: Null type throws ArgumentException")]
    [TestCase("", Description = "Domain-012: Empty type throws ArgumentException")]
    public void Constructor_InvalidType_ThrowsArgumentException(string? invalidType)
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
            Assert.That(caughtException!.Message, Does.Contain("Type"));
        });
    }

    /// <summary>
    /// Domain-003, Domain-004, Domain-005: Verify that creating a LearningSpace with a zero
    /// dimension throws ArgumentException.
    /// </summary>
    [TestCase(0.0f, ValidWidth, ValidLength, "Height", Description = "Domain-003: Zero height throws ArgumentException")]
    [TestCase(ValidHeight, 0.0f, ValidLength, "Width", Description = "Domain-004: Zero width throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, 0.0f, "Length", Description = "Domain-005: Zero length throws ArgumentException")]
    public void Constructor_ZeroDimension_ThrowsArgumentException(
        float height, float width, float length, string expectedDimension)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidTypeClassroom, height, width, length);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedDimension));
        });
    }

    /// <summary>
    /// Domain-006, Domain-007, Domain-008: Verify that creating a LearningSpace with a negative
    /// dimension throws ArgumentException.
    /// </summary>
    [TestCase(-3.0f, ValidWidth, ValidLength, "Height", Description = "Domain-006: Negative height throws ArgumentException")]
    [TestCase(ValidHeight, -8.0f, ValidLength, "Width", Description = "Domain-007: Negative width throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, -10.0f, "Length", Description = "Domain-008: Negative length throws ArgumentException")]
    public void Constructor_NegativeDimension_ThrowsArgumentException(
        float height, float width, float length, string expectedDimension)
    {
        // Arrange & Act
        ArgumentException? caughtException = null;
        try
        {
            new LearningSpace(ValidTypeClassroom, height, width, length);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.Message, Does.Contain(expectedDimension));
        });
    }
}
