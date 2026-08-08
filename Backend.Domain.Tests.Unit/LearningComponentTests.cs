using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Tests.Unit;

/// <summary>
/// Unit tests for the <see cref="LearningComponent"/> entity.
/// Covers constructor validation, property assignment, and boundary values.
/// </summary>
[TestFixture]
public class LearningComponentTests
{
    // ── Valid default values used across tests ──────────────────────────
    private const string ValidComponentId = "COMP-001";
    private const string ValidLearningSpaceId = "IF-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.5f;
    private const float ValidX = 1.0f;
    private const float ValidY = 2.0f;
    private const float ValidZ = 0.0f;
    private const string ValidOrientation = "North";

    // ────────────────────────────────────────────────────────────────────
    // Domain-001  –  Constructor with valid parameters
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Domain-001: Verify that a LearningComponent entity can be created with valid parameters and all properties are set correctly.")]
    public void Constructor_ValidParameters_SetsAllProperties()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
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

    // ────────────────────────────────────────────────────────────────────
    // Domain-002 to Domain-007  –  Negative dimensions / coordinates
    // ────────────────────────────────────────────────────────────────────

    [TestCaseSource(nameof(NegativeDimensionAndCoordinateCases))]
    [Description("Domain-002 to Domain-007: Verify that creating a LearningComponent with a negative dimension or coordinate throws ArgumentException with the correct ParamName.")]
    public void Constructor_NegativeDimensionOrCoordinate_ThrowsArgumentException(
        float width, float height, float depth,
        float x, float y, float z,
        string expectedParamName)
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                width, height, depth,
                x, y, z,
                ValidOrientation));

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo(expectedParamName));
    }

    /// <summary>
    /// Provides one test case per negative-valued parameter (width, height, depth, x, y, z).
    /// All other parameters are kept at their valid defaults.
    /// </summary>
    private static IEnumerable<TestCaseData> NegativeDimensionAndCoordinateCases()
    {
        yield return new TestCaseData(-1f, ValidHeight, ValidDepth, ValidX, ValidY, ValidZ, "width")
            .SetName("Negative width throws ArgumentException");
        yield return new TestCaseData(ValidWidth, -1f, ValidDepth, ValidX, ValidY, ValidZ, "height")
            .SetName("Negative height throws ArgumentException");
        yield return new TestCaseData(ValidWidth, ValidHeight, -1f, ValidX, ValidY, ValidZ, "depth")
            .SetName("Negative depth throws ArgumentException");
        yield return new TestCaseData(ValidWidth, ValidHeight, ValidDepth, -1f, ValidY, ValidZ, "x")
            .SetName("Negative X coordinate throws ArgumentException");
        yield return new TestCaseData(ValidWidth, ValidHeight, ValidDepth, ValidX, -1f, ValidZ, "y")
            .SetName("Negative Y coordinate throws ArgumentException");
        yield return new TestCaseData(ValidWidth, ValidHeight, ValidDepth, ValidX, ValidY, -1f, "z")
            .SetName("Negative Z coordinate throws ArgumentException");
    }

    // ────────────────────────────────────────────────────────────────────
    // Domain-008  –  Invalid orientation
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Domain-008: Verify that creating a LearningComponent with an invalid orientation throws ArgumentException.")]
    public void Constructor_InvalidOrientation_ThrowsArgumentException()
    {
        // Arrange & Act
        var ex = Assert.Throws<ArgumentException>(() =>
            new LearningComponent(
                ValidComponentId, ValidLearningSpaceId,
                ValidWidth, ValidHeight, ValidDepth,
                ValidX, ValidY, ValidZ,
                "InvalidOrientation"));

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("orientation"));
    }

    // ────────────────────────────────────────────────────────────────────
    // Domain-009  –  Valid orientation values
    // ────────────────────────────────────────────────────────────────────

    [TestCase("North", Description = "North orientation is accepted")]
    [TestCase("South", Description = "South orientation is accepted")]
    [TestCase("East", Description = "East orientation is accepted")]
    [TestCase("West", Description = "West orientation is accepted")]
    [Description("Domain-009: Verify that creating a LearningComponent with each valid orientation (North, South, East, West) succeeds.")]
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

    // ────────────────────────────────────────────────────────────────────
    // Domain-010  –  Zero-value boundary test
    // ────────────────────────────────────────────────────────────────────

    [Test]
    [Description("Domain-010: Verify that creating a LearningComponent with zero values for all dimensions and coordinates succeeds (boundary test).")]
    public void Constructor_ZeroValuesForDimensionsAndCoordinates_SetsPropertiesToZero()
    {
        // Arrange & Act
        var component = new LearningComponent(
            ValidComponentId, ValidLearningSpaceId,
            0f, 0f, 0f,
            0f, 0f, 0f,
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
