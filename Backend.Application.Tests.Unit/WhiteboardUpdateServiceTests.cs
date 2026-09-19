using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.UpdateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-005.
/// </summary>
[TestFixture]
public class WhiteboardUpdateServiceTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceRepository> _mockLearningSpaceRepository = null!;

    // Valid test data
    private const string ValidWhiteboardId = "WB-001";
    private const string ValidLearningSpaceId = "LS-001";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "South";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceRepository = new Mock<ILearningSpaceRepository>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceRepository.VerifyAll();
    }

    private static Whiteboard CreateExistingWhiteboard()
    {
        return new Whiteboard(
            ValidWhiteboardId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    private static UpdateWhiteboardDto CreateValidUpdateDto(
        string? markerColor = null,
        float? width = null,
        float? height = null,
        float? depth = null,
        float? x = null,
        float? y = null,
        float? z = null,
        string? orientation = null)
    {
        return new UpdateWhiteboardDto(
            ValidWhiteboardId,
            width ?? ValidWidth,
            height ?? ValidHeight,
            depth ?? ValidDepth,
            x ?? ValidX,
            y ?? ValidY,
            z ?? ValidZ,
            orientation ?? ValidOrientation,
            markerColor ?? ValidMarkerColor);
    }

    /// <summary>
    /// Application-001: Verify that the service successfully updates a whiteboard
    /// when valid data is provided.
    /// </summary>
    [Test]
    [Description("Application-001: Service successfully updates whiteboard with valid data")]
    public async Task UpdateWhiteboardAsync_ValidData_ReturnsSuccessAndCallsUpdate()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockWhiteboardRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        var service = new WhiteboardService(_mockWhiteboardRepository.Object);
        var updateDto = CreateValidUpdateDto(markerColor: "Red", width: 3.0f, height: 2.0f, depth: 0.2f);

        // Act
        var result = await service.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.True);
        });
        _mockWhiteboardRepository.Verify(
            r => r.UpdateAsync(It.Is<Whiteboard>(w => w.MarkerColor == "Red")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that the service returns a failure result
    /// when the whiteboard with the given ID does not exist.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns failure when whiteboard ID is not found")]
    public async Task UpdateWhiteboardAsync_WhiteboardNotFound_ReturnsFailure()
    {
        // Arrange
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync((Whiteboard?)null);

        var service = new WhiteboardService(_mockWhiteboardRepository.Object);
        var updateDto = CreateValidUpdateDto(markerColor: "Red");

        // Act
        var result = await service.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Whiteboard not found"));
        });
        _mockWhiteboardRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Whiteboard>()),
            Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service returns a failure result
    /// when validation fails (invalid markerColor).
    /// </summary>
    [Test]
    [Description("Application-003: Service returns failure when validation fails for marker color")]
    public async Task UpdateWhiteboardAsync_InvalidMarkerColor_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);

        var service = new WhiteboardService(_mockWhiteboardRepository.Object);
        var updateDto = CreateValidUpdateDto(markerColor: "");

        // Act
        var result = await service.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Invalid marker color"));
        });
        _mockWhiteboardRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Whiteboard>()),
            Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that the service returns a failure result
    /// when the new position exceeds learning space boundaries.
    /// </summary>
    [Test]
    [Description("Application-004: Service returns failure when position exceeds learning space boundaries")]
    public async Task UpdateWhiteboardAsync_PositionExceedsBoundaries_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace(ValidLearningSpaceId, "Classroom", 3.0f, 5.0f, 5.0f));

        var service = new WhiteboardService(_mockWhiteboardRepository.Object, _mockLearningSpaceRepository.Object);
        var updateDto = CreateValidUpdateDto(x: 10.0f, z: 10.0f);

        // Act
        var result = await service.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position exceeds learning space boundaries"));
        });
        _mockWhiteboardRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Whiteboard>()),
            Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service returns a failure result
    /// when the new position overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Application-005: Service returns failure when position overlaps with existing component")]
    public async Task UpdateWhiteboardAsync_PositionOverlapsWithExistingComponent_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        var otherComponentId = "WB-002";
        var otherComponent = new Whiteboard(
            otherComponentId, ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "East", "Green");

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidWhiteboardId))
            .ReturnsAsync(existingWhiteboard);
        _mockWhiteboardRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent> { otherComponent });
        _mockLearningSpaceRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace(ValidLearningSpaceId, "Classroom", 3.0f, 10.0f, 10.0f));

        var service = new WhiteboardService(_mockWhiteboardRepository.Object, _mockLearningSpaceRepository.Object);
        var updateDto = CreateValidUpdateDto(x: 5.0f, z: 5.0f);

        // Act
        var result = await service.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position overlaps with existing component"));
        });
        _mockWhiteboardRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Whiteboard>()),
            Times.Never);
    }
}
