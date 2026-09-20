using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="Whiteboard.Update"/> method.
/// Covers intents Domain-012 through Domain-014 for story CPD-LC-001-005.
/// </summary>
[TestFixture]
public class WhiteboardUpdateTests
{
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    private Whiteboard _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _sut = new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    /// <summary>
    /// Domain-012: Verify that Update method successfully updates all whiteboard
    /// properties when provided with valid values.
    /// </summary>
    [Test]
    [Description("Domain-012: Update method successfully updates whiteboard properties with valid values")]
    public void Update_ValidValues_AllPropertiesUpdated()
    {
        // Arrange
        var newMarkerColor = "Red";
        var newWidth = 3.0f;
        var newHeight = 2.0f;
        var newDepth = 0.2f;
        var newX = 2.0f;
        var newY = 1.0f;
        var newZ = 3.0f;
        var newOrientation = "South";

        // Act
        _sut.Update(newWidth, newHeight, newDepth, newX, newY, newZ, newOrientation, newMarkerColor);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(_sut.MarkerColor, Is.EqualTo(newMarkerColor));
            Assert.That(_sut.Width, Is.EqualTo(newWidth));
            Assert.That(_sut.Height, Is.EqualTo(newHeight));
            Assert.That(_sut.Depth, Is.EqualTo(newDepth));
            Assert.That(_sut.X, Is.EqualTo(newX));
            Assert.That(_sut.Y, Is.EqualTo(newY));
            Assert.That(_sut.Z, Is.EqualTo(newZ));
            Assert.That(_sut.Orientation, Is.EqualTo(newOrientation));
        });
    }

    /// <summary>
    /// Domain-013: Verify that Update throws InvalidOperationException when
    /// the new position exceeds the learning space boundaries.
    /// </summary>
    [Test]
    [Description("Domain-013: Update throws InvalidOperationException when position exceeds learning space boundaries")]
    public void Update_PositionExceedsLearningSpaceBoundaries_ThrowsInvalidOperationException()
    {
        // Arrange
        var learningSpaceWidth = 10.0f;
        var learningSpaceLength = 10.0f;
        var exceedingX = 15.0f;
        var exceedingZ = 15.0f;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _sut.Update(ValidWidth, ValidHeight, ValidDepth,
                exceedingX, ValidY, exceedingZ,
                ValidOrientation, ValidMarkerColor,
                learningSpaceWidth, learningSpaceLength));

        Assert.That(ex!.Message, Does.Contain("Position exceeds learning space boundaries"));
    }

    /// <summary>
    /// Domain-014: Verify that Update throws InvalidOperationException when
    /// the new position overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Domain-014: Update throws InvalidOperationException when position overlaps with existing component")]
    public void Update_PositionOverlapsWithExistingComponent_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingComponents = new List<LearningComponent>
        {
            new Whiteboard(
                "WB-002", ValidLearningSpaceId,
                2.0f, 1.5f, 0.1f,
                3.0f, 0.0f, 4.0f,
                "South", "Green")
        };
        var overlappingX = 3.0f;
        var overlappingZ = 4.0f;

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _sut.Update(ValidWidth, ValidHeight, ValidDepth,
                overlappingX, ValidY, overlappingZ,
                ValidOrientation, ValidMarkerColor,
                existingComponents));

        Assert.That(ex!.Message, Does.Contain("Position overlaps with existing component"));
    }
}
