using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
/// Covers constructor validation, property assignment, and boundary conditions.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    private const string ValidComponentId = "comp-001";
    private const string ValidLearningSpaceId = "ls-001";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 3.0f;
    private const float ValidDepth = 1.5f;
    private const float ValidX = 10.0f;
    private const float ValidY = 20.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    /// <summary>
    /// Domain-001: Verify that a LearningComponent entity can be created with valid parameters
    /// and all properties are correctly assigned.
    /// </summary>
    [Test]
    [Description("Domain-001: Constructor with valid parameters sets all properties correctly")]
    public void Constructor_ValidParameters_SetsAllProperties()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId,
            ValidLearningSpaceId,
            ValidWidth,
            ValidHeight,
            ValidDepth,
            ValidX,
            ValidY,
            ValidZ,
            ValidOrientation);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(component.ComponentId, Is.EqualTo(ValidComponentId));
            Assert.That(component.LearningSpaceId, Is.EqualTo(ValidLearningSpaceId));
            Assert.That(component.Width, Is.EqualTo(ValidWidth));
            Assert.That(component.Height, Is.EqualTo(ValidHeight));
            Assert.That(component.Depth, Is.EqualTo(ValidDepth));
            Assert.That(component.X, Is.EqualTo(ValidX));
            Assert.That(component.Y, Is.EqualTo(ValidY));
            Assert.That(component.Z, Is.EqualTo(ValidZ));
            Assert.That(component.Orientation, Is.EqualTo(ValidOrientation));
        });
    }

    /// <summary>
    /// Domain-002 to Domain-007: Verify that creating a LearningComponent with a negative
    /// dimension or coordinate throws an ArgumentException with the correct parameter name.
    /// </summary>
    [TestCaseSource(nameof(NegativeDimensionAndCoordinateTestCases))]
    [Description("Domain-002 to Domain-007: Negative dimensions and coordinates throw ArgumentException")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width,
        float height,
        float depth,
        float x,
        float y,
        float z,
        string expectedParamName)
    {
        // Arrange, Act & Assert
        Assert.That(
            () => new LearningComponent(
                ValidComponentId,
                ValidLearningSpaceId,
                width,
                height,
                depth,
                x,
                y,
                z,
                ValidOrientation),
            Throws.ArgumentException.With.Property("ParamName").EqualTo(expectedParamName));
    }

    /// <summary>
    /// Test case source for negative dimension and coordinate validation tests.
    /// Each test case isolates one parameter as negative while keeping all others valid.
    /// </summary>
    private static IEnumerable<TestCaseData> NegativeDimensionAndCoordinateTestCases
    {
        get
        {
            // Domain-002: Negative width
            yield return new TestCaseData(
                    -1.0f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width")
                .SetName("Constructor_NegativeWidth_ThrowsArgumentException");

            // Domain-003: Negative height
            yield return new TestCaseData(
                    ValidWidth, -1.0f, ValidDepth, ValidX, ValidY, ValidZ, "height")
                .SetName("Constructor_NegativeHeight_ThrowsArgumentException");

            // Domain-004: Negative depth
            yield return new TestCaseData(
                    ValidWidth, ValidHeight, -1.0f, ValidX, ValidY, ValidZ, "depth")
                .SetName("Constructor_NegativeDepth_ThrowsArgumentException");

            // Domain-005: Negative X coordinate
            yield return new TestCaseData(
                    ValidWidth, ValidHeight, ValidDepth, -1.0f, ValidY, ValidZ, "x")
                .SetName("Constructor_NegativeX_ThrowsArgumentException");

            // Domain-006: Negative Y coordinate
            yield return new TestCaseData(
                    ValidWidth, ValidHeight, ValidDepth, ValidX, -1.0f, ValidZ, "y")
                .SetName("Constructor_NegativeY_ThrowsArgumentException");

            // Domain-007: Negative Z coordinate
            yield return new TestCaseData(
                    ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1.0f, "z")
                .SetName("Constructor_NegativeZ_ThrowsArgumentException");
        }
    }

    /// <summary>
    /// Domain-008: Verify that creating a LearningComponent with an invalid orientation
    /// throws an ArgumentException with parameter name "orientation".
    /// </summary>
    [Test]
    [Description("Domain-008: Invalid orientation throws ArgumentException")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange
        const string invalidOrientation = "Northeast";

        // Act & Assert
        Assert.That(
            () => new LearningComponent(
                ValidComponentId,
                ValidLearningSpaceId,
                ValidWidth,
                ValidHeight,
                ValidDepth,
                ValidX,
                ValidY,
                ValidZ,
                invalidOrientation),
            Throws.ArgumentException.With.Property("ParamName").EqualTo("orientation"));
    }

    /// <summary>
    /// Domain-009: Verify that creating a LearningComponent with each valid orientation
    /// (North, South, East, West) succeeds and sets the orientation correctly.
    /// </summary>
    [TestCase("North", Description = "Valid orientation: North")]
    [TestCase("South", Description = "Valid orientation: South")]
    [TestCase("East", Description = "Valid orientation: East")]
    [TestCase("West", Description = "Valid orientation: West")]
    public void Constructor_ValidOrientation_SetsOrientation(string orientation)
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId,
            ValidLearningSpaceId,
            ValidWidth,
            ValidHeight,
            ValidDepth,
            ValidX,
            ValidY,
            ValidZ,
            orientation);

        // Assert
        Assert.That(component.Orientation, Is.EqualTo(orientation));
    }

    /// <summary>
    /// Domain-010: Verify that creating a LearningComponent with zero values for all
    /// dimensions and coordinates succeeds (boundary test). Zero is the minimum valid value.
    /// </summary>
    [Test]
    [Description("Domain-010: Zero values for dimensions and coordinates succeed (boundary test)")]
    public void Constructor_ZeroValues_SetsAllPropertiesToZero()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId,
            ValidLearningSpaceId,
            0f,
            0f,
            0f,
            0f,
            0f,
            0f,
            ValidOrientation);

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
