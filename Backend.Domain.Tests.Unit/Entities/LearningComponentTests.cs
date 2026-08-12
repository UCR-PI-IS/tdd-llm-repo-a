using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit.Entities;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity constructor and validation.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private const string ValidComponentId = "C001";
    private const string ValidLearningSpaceId = "LS001";
    private const float ValidWidth = 2.5f;
    private const float ValidHeight = 1.8f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 2.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Test case source for negative dimension and coordinate values.
    /// Each test case provides the full constructor arguments with one invalid (negative) value
    /// and the expected parameter name that should appear in the ArgumentException.
    /// </summary>
    public static IEnumerable<TestCaseData> NegativeDimensionOrCoordinateCases
    {
        get
        {
            // Negative width
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                -1.0f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation,
                "width"
            ).SetName("NegativeWidth");

            // Negative height
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, -1.0f, ValidDepth, ValidX, ValidY, ValidZ, ValidOrientation,
                "height"
            ).SetName("NegativeHeight");

            // Negative depth
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, -1.0f, ValidX, ValidY, ValidZ, ValidOrientation,
                "depth"
            ).SetName("NegativeDepth");

            // Negative X coordinate
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth, -1.0f, ValidY, ValidZ, ValidOrientation,
                "x"
            ).SetName("NegativeX");

            // Negative Y coordinate
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth, ValidX, -1.0f, ValidZ, ValidOrientation,
                "y"
            ).SetName("NegativeY");

            // Negative Z coordinate
            yield return new TestCaseData(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1.0f, ValidOrientation,
                "z"
            ).SetName("NegativeZ");
        }
    }

    [Test(Description = "Verify that a LearningComponent entity can be created with valid parameters and all properties are set correctly")]
    public void Constructor_ValidParameters_SetsAllProperties()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var width = ValidWidth;
        var height = ValidHeight;
        var depth = ValidDepth;
        var x = ValidX;
        var y = ValidY;
        var z = ValidZ;
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

    [TestCaseSource(nameof(NegativeDimensionOrCoordinateCases))]
    [Test(Description = "Verify that creating a LearningComponent with a negative dimension or coordinate throws ArgumentException with the correct parameter name")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        string componentId, string learningSpaceId,
        float width, float height, float depth,
        float x, float y, float z,
        string orientation, string expectedParamName)
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation));

        Assert.That(ex!.ParamName, Is.EqualTo(expectedParamName));
    }

    [Test(Description = "Verify that creating a LearningComponent with an invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var invalidOrientation = "InvalidDirection";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                invalidOrientation));

        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    [TestCase("North", Description = "Verify LearningComponent accepts 'North' orientation")]
    [TestCase("South", Description = "Verify LearningComponent accepts 'South' orientation")]
    [TestCase("East", Description = "Verify LearningComponent accepts 'East' orientation")]
    [TestCase("West", Description = "Verify LearningComponent accepts 'West' orientation")]
    public void Constructor_ValidOrientation_SetsOrientation(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    [Test(Description = "Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds (boundary test)")]
    public void Constructor_ZeroValues_SetsAllToZero()
    {
        // Arrange
        var componentId = ValidComponentId;
        var learningSpaceId = ValidLearningSpaceId;
        var zeroWidth = 0f;
        var zeroHeight = 0f;
        var zeroDepth = 0f;
        var zeroX = 0f;
        var zeroY = 0f;
        var zeroZ = 0f;
        var orientation = ValidOrientation;

        // Act
        var component = new LearningComponent(
            componentId, learningSpaceId,
            zeroWidth, zeroHeight, zeroDepth,
            zeroX, zeroY, zeroZ,
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
