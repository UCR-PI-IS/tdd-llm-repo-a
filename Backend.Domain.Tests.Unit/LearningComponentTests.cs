using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
/// Covers constructor validation, property assignment, and boundary conditions.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private const string DefaultComponentId = "COMP-001";
    private const string DefaultLearningSpaceId = "LS-001";
    private const float DefaultWidth = 2f;
    private const float DefaultHeight = 3f;
    private const float DefaultDepth = 1f;
    private const float DefaultX = 5f;
    private const float DefaultY = 10f;
    private const float DefaultZ = 0f;
    private const string DefaultOrientation = "North";

    /// <summary>
    /// Test case source for negative dimension and coordinate validation.
    /// Each case provides (width, height, depth, x, y, z, expectedParamName).
    /// Covers Domain-002 through Domain-007.
    /// </summary>
    private static readonly object[] NegativeDimensionAndCoordinateCases =
    {
        new object[] { -1f, 1f, 1f, 1f, 1f, 1f, "width" },
        new object[] { 1f, -1f, 1f, 1f, 1f, 1f, "height" },
        new object[] { 1f, 1f, -1f, 1f, 1f, 1f, "depth" },
        new object[] { 1f, 1f, 1f, -1f, 1f, 1f, "x" },
        new object[] { 1f, 1f, 1f, 1f, -1f, 1f, "y" },
        new object[] { 1f, 1f, 1f, 1f, 1f, -1f, "z" },
    };

    [Test]
    [Description("Verify that a LearningComponent entity can be created with valid parameters and all properties are set correctly (Domain-001)")]
    public void Constructor_ValidParameters_PropertiesSetCorrectly()
    {
        // Arrange
        var componentId = DefaultComponentId;
        var learningSpaceId = DefaultLearningSpaceId;
        var width = DefaultWidth;
        var height = DefaultHeight;
        var depth = DefaultDepth;
        var x = DefaultX;
        var y = DefaultY;
        var z = DefaultZ;
        var orientation = DefaultOrientation;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

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

    [Test, TestCaseSource(nameof(NegativeDimensionAndCoordinateCases))]
    [Description("Verify that creating a LearningComponent with a negative dimension or coordinate throws ArgumentException with the correct parameter name (Domain-002 through Domain-007)")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth, float x, float y, float z, string expectedParamName)
    {
        // Arrange
        var componentId = DefaultComponentId;
        var learningSpaceId = DefaultLearningSpaceId;
        var orientation = DefaultOrientation;

        // Act & Assert
        Assert.That(
            () => new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation),
            Throws.ArgumentException.With.Property("ParamName").EqualTo(expectedParamName));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with an invalid orientation throws ArgumentException (Domain-008)")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        var componentId = DefaultComponentId;
        var learningSpaceId = DefaultLearningSpaceId;
        var width = DefaultWidth;
        var height = DefaultHeight;
        var depth = DefaultDepth;
        var x = DefaultX;
        var y = DefaultY;
        var z = DefaultZ;
        var invalidOrientation = "InvalidDirection";

        // Act & Assert
        Assert.That(
            () => new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, invalidOrientation),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("orientation"));
    }

    [TestCase("North", Description = "Verify North orientation is accepted (Domain-009)")]
    [TestCase("South", Description = "Verify South orientation is accepted (Domain-009)")]
    [TestCase("East", Description = "Verify East orientation is accepted (Domain-009)")]
    [TestCase("West", Description = "Verify West orientation is accepted (Domain-009)")]
    public void Constructor_ValidOrientation_Succeeds(string orientation)
    {
        // Arrange
        var componentId = DefaultComponentId;
        var learningSpaceId = DefaultLearningSpaceId;
        var width = DefaultWidth;
        var height = DefaultHeight;
        var depth = DefaultDepth;
        var x = DefaultX;
        var y = DefaultY;
        var z = DefaultZ;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    [Test]
    [Description("Verify that creating a LearningComponent with zero values for dimensions and coordinates succeeds as a boundary test (Domain-010)")]
    public void Constructor_ZeroValues_Succeeds()
    {
        // Arrange
        var componentId = DefaultComponentId;
        var learningSpaceId = DefaultLearningSpaceId;
        var width = 0f;
        var height = 0f;
        var depth = 0f;
        var x = 0f;
        var y = 0f;
        var z = 0f;
        var orientation = DefaultOrientation;

        // Act
        var component = new LearningComponent(componentId, learningSpaceId, width, height, depth, x, y, z, orientation);

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
