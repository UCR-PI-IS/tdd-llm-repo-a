using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor and validation.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    // ── Valid base values shared across test cases ──
    private const string ValidComponentId = "COMP-001";
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidDimension = 1.5f;
    private const float ValidCoordinate = 2.0f;
    private const string ValidOrientation = "North";
    private const float NegativeValue = -1.0f;

    /// <summary>
    /// Test case source for Domain-002 through Domain-007.
    /// Each case sets exactly one numeric parameter to a negative value
    /// and specifies the expected <see cref="ArgumentException.ParamName"/>.
    /// </summary>
    public static IEnumerable<TestCaseData> NegativeParameterTestCases
    {
        get
        {
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                NegativeValue, ValidDimension, ValidDimension,
                ValidCoordinate, ValidCoordinate, ValidCoordinate,
                ValidOrientation,
                "width"
            ).SetName("Constructor_NegativeWidth_ThrowsArgumentException");

            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidDimension, NegativeValue, ValidDimension,
                ValidCoordinate, ValidCoordinate, ValidCoordinate,
                ValidOrientation,
                "height"
            ).SetName("Constructor_NegativeHeight_ThrowsArgumentException");

            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidDimension, ValidDimension, NegativeValue,
                ValidCoordinate, ValidCoordinate, ValidCoordinate,
                ValidOrientation,
                "depth"
            ).SetName("Constructor_NegativeDepth_ThrowsArgumentException");

            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidDimension, ValidDimension, ValidDimension,
                NegativeValue, ValidCoordinate, ValidCoordinate,
                ValidOrientation,
                "x"
            ).SetName("Constructor_NegativeX_ThrowsArgumentException");

            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidDimension, ValidDimension, ValidDimension,
                ValidCoordinate, NegativeValue, ValidCoordinate,
                ValidOrientation,
                "y"
            ).SetName("Constructor_NegativeY_ThrowsArgumentException");

            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidDimension, ValidDimension, ValidDimension,
                ValidCoordinate, ValidCoordinate, NegativeValue,
                ValidOrientation,
                "z"
            ).SetName("Constructor_NegativeZ_ThrowsArgumentException");
        }
    }

    /// <summary>
    /// Domain-001: Verifies that a LearningComponent entity can be created
    /// with valid parameters and all properties are set correctly.
    /// </summary>
    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters")]
    public void Constructor_ValidParameters_AllPropertiesSetCorrectly()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidDimension;
        var height = 3.0f;
        var depth = 2.5f;
        var x = ValidCoordinate;
        var y = 4.0f;
        var z = 1.0f;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.EqualTo(componentId));
            Assert.That(component.LearningSpaceId, Is.EqualTo(learningSpaceId));
            Assert.That(component.Width, Is.EqualTo(width));
            Assert.That(component.Height, Is.EqualTo(height));
            Assert.That(component.Depth, Is.EqualTo(depth));
            Assert.That(component.X, Is.EqualTo(x));
            Assert.That(component.Y, Is.EqualTo(y));
            Assert.That(component.Z, Is.EqualTo(z));
            Assert.That(component.Orientation, Is.EqualTo(orientation));
        });
    }

    /// <summary>
    /// Domain-002 through Domain-007: Verifies that creating a LearningComponent
    /// with a negative value for any dimension or coordinate throws ArgumentException
    /// with the correct parameter name.
    /// </summary>
    [Test]
    [TestCaseSource(nameof(NegativeParameterTestCases))]
    [Description("Verify that creating a LearningComponent with a negative dimension or coordinate throws ArgumentException")]
    public void Constructor_NegativeParameter_ThrowsArgumentException(
        string componentId, string learningSpaceId,
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string expectedParamName)
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new LearningComponent(
                    componentId, learningSpaceId,
                    width, height, depth,
                    x, y, z,
                    orientation));
            Assert.That(ex!.ParamName, Is.EqualTo(expectedParamName));
        });
    }

    /// <summary>
    /// Domain-008: Verifies that creating a LearningComponent with an invalid
    /// orientation value throws ArgumentException.
    /// </summary>
    [Test]
    [TestCase("Northeast", Description = "Invalid orientation 'Northeast' throws ArgumentException")]
    [TestCase("Up", Description = "Invalid orientation 'Up' throws ArgumentException")]
    [TestCase("", Description = "Empty orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException(string invalidOrientation)
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new LearningComponent(
                    ValidComponentId, ValidLearningSpaceId,
                    ValidDimension, ValidDimension, ValidDimension,
                    ValidCoordinate, ValidCoordinate, ValidCoordinate,
                    invalidOrientation));
            Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
        });
    }

    /// <summary>
    /// Domain-009: Verifies that creating a LearningComponent with each valid
    /// orientation (North, South, East, West) succeeds and stores the correct value.
    /// </summary>
    [Test]
    [TestCase("North", Description = "Valid orientation 'North' creates successfully")]
    [TestCase("South", Description = "Valid orientation 'South' creates successfully")]
    [TestCase("East", Description = "Valid orientation 'East' creates successfully")]
    [TestCase("West", Description = "Valid orientation 'West' creates successfully")]
    public void Constructor_ValidOrientation_CreatesSuccessfully(string orientation)
    {
        // Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            ValidDimension, ValidDimension, ValidDimension,
            ValidCoordinate, ValidCoordinate, ValidCoordinate,
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Domain-010: Verifies that creating a LearningComponent with zero values
    /// for all dimensions and coordinates succeeds (boundary test).
    /// </summary>
    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds")]
    public void Constructor_ZeroValuesBoundary_AllPropertiesSetToZero()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var zeroValue = 0f;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId,
            zeroValue, zeroValue, zeroValue,
            zeroValue, zeroValue, zeroValue,
            orientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.Width, Is.EqualTo(0f));
            Assert.That(component.Height, Is.EqualTo(0f));
            Assert.That(component.Depth, Is.EqualTo(0f));
            Assert.That(component.X, Is.EqualTo(0f));
            Assert.That(component.Y, Is.EqualTo(0f));
            Assert.That(component.Z, Is.EqualTo(0f));
        });
    }
}
