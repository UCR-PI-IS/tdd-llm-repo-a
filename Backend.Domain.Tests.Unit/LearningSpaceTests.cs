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
    private const float ValidHeight = 3.0f;
    private const float ValidWidth = 8.0f;
    private const float ValidLength = 10.0f;

    /// <summary>
    /// Domain-001, Domain-009, Domain-010: Verify that a LearningSpace entity can be created
    /// with each valid type (Classroom, Auditorium, Laboratory) and all properties are correctly assigned,
    /// including an auto-generated internal ID greater than zero.
    /// </summary>
    [TestCase("Classroom", 3.0f, 8.0f, 10.0f,
        Description = "Domain-001: Valid Classroom creation with all properties and auto-generated ID")]
    [TestCase("Auditorium", 5.0f, 15.0f, 20.0f,
        Description = "Domain-009: Valid Auditorium creation with all properties and auto-generated ID")]
    [TestCase("Laboratory", 3.5f, 12.0f, 15.0f,
        Description = "Domain-010: Valid Laboratory creation with all properties and auto-generated ID")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly(
        string type, float height, float width, float length)
    {
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
    /// Domain-002: Verify that creating a LearningSpace with an invalid type
    /// throws ArgumentException with the appropriate message.
    /// </summary>
    [Test]
    [Description("Domain-002: Invalid type throws ArgumentException")]
    public void Constructor_InvalidType_ThrowsArgumentException()
    {
        // Arrange
        var invalidType = "InvalidType";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(invalidType, ValidHeight, ValidWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Type must be Classroom, Auditorium, or Laboratory"));
    }

    /// <summary>
    /// Domain-003 through Domain-008: Verify that creating a LearningSpace with zero or negative
    /// dimensions throws ArgumentException with the appropriate message for each dimension.
    /// </summary>
    [TestCase(0.0f, ValidWidth, ValidLength, "Height must be positive and non-zero",
        Description = "Domain-003: Zero height throws ArgumentException")]
    [TestCase(-3.0f, ValidWidth, ValidLength, "Height must be positive and non-zero",
        Description = "Domain-006: Negative height throws ArgumentException")]
    [TestCase(ValidHeight, 0.0f, ValidLength, "Width must be positive and non-zero",
        Description = "Domain-004: Zero width throws ArgumentException")]
    [TestCase(ValidHeight, -8.0f, ValidLength, "Width must be positive and non-zero",
        Description = "Domain-007: Negative width throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, 0.0f, "Length must be positive and non-zero",
        Description = "Domain-005: Zero length throws ArgumentException")]
    [TestCase(ValidHeight, ValidWidth, -10.0f, "Length must be positive and non-zero",
        Description = "Domain-008: Negative length throws ArgumentException")]
    public void Constructor_InvalidDimension_ThrowsArgumentException(
        float height, float width, float length, string expectedMessage)
    {
        // Arrange
        var type = "Classroom";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(type, height, width, length));
        Assert.That(ex!.Message, Does.Contain(expectedMessage));
    }

    /// <summary>
    /// Domain-011 and Domain-012: Verify that creating a LearningSpace with a null or empty
    /// type throws ArgumentException with the appropriate message.
    /// </summary>
    [TestCase(null, Description = "Domain-011: Null type throws ArgumentException")]
    [TestCase("", Description = "Domain-012: Empty type throws ArgumentException")]
    public void Constructor_NullOrEmptyType_ThrowsArgumentException(string? invalidType)
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningSpace(invalidType!, ValidHeight, ValidWidth, ValidLength));
        Assert.That(ex!.Message, Does.Contain("Type is required"));
    }
}
