using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="WhiteboardService.UpdateWhiteboardAsync"/>.
/// Covers intents Application-001 through Application-005 for story CPD-LC-001-005.
/// </summary>
[TestFixture]
public class WhiteboardServiceUpdateTests
{
    private Mock<IWhiteboardRepository> _mockWhiteboardRepository = null!;
    private Mock<ILearningSpaceReadRepository> _mockLearningSpaceReadRepository = null!;
    private WhiteboardService _sut = null!;

    // Valid test data
    private const string ValidComponentId = "WB-001";
    private const string ValidLearningSpaceId = "LS-0103";
    private const float ValidWidth = 2.0f;
    private const float ValidHeight = 1.5f;
    private const float ValidDepth = 0.1f;
    private const float ValidX = 1.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 2.0f;
    private const string ValidOrientation = "North";
    private const string ValidMarkerColor = "Blue";

    [SetUp]
    public void SetUp()
    {
        _mockWhiteboardRepository = new Mock<IWhiteboardRepository>();
        _mockLearningSpaceReadRepository = new Mock<ILearningSpaceReadRepository>();
        _sut = new WhiteboardService(
            _mockWhiteboardRepository.Object,
            _mockLearningSpaceReadRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockWhiteboardRepository.VerifyAll();
        _mockLearningSpaceReadRepository.VerifyAll();
    }

    private static Whiteboard CreateExistingWhiteboard()
    {
        return new Whiteboard(
            ValidComponentId, ValidLearningSpaceId,
            ValidWidth, ValidHeight, ValidDepth,
            ValidX, ValidY, ValidZ,
            ValidOrientation, ValidMarkerColor);
    }

    private static UpdateWhiteboardDto CreateUpdateDto(
        string? componentId = null,
        float width = 3.0f,
        float height = 2.0f,
        float depth = 0.2f,
        float x = 2.0f,
        float y = 1.0f,
        float z = 3.0f,
        string orientation = "South",
        string markerColor = "Red")
    {
        return new UpdateWhiteboardDto(
            componentId ?? ValidComponentId,
            width, height, depth, x, y, z,
            orientation, markerColor);
    }

    /// <summary>
    /// Application-001: Verify that the service successfully updates a whiteboard
    /// when valid data is provided, persists the changes, and returns a success result.
    /// </summary>
    [Test]
    [Description("Application-001: Service successfully updates whiteboard with valid data")]
    public async Task UpdateWhiteboardAsync_ValidData_ReturnsSuccessAndPersistsChanges()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidComponentId))
            .ReturnsAsync(existingWhiteboard);
        _mockWhiteboardRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Whiteboard>()))
            .Returns(Task.CompletedTask);

        var updateDto = CreateUpdateDto();

        // Act
        var result = await _sut.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
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
    [Description("Application-002: Service returns failure when whiteboard not found")]
    public async Task UpdateWhiteboardAsync_WhiteboardNotFound_ReturnsFailure()
    {
        // Arrange
        var nonExistentId = "WB-NONEXISTENT";
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Whiteboard?)null);

        var updateDto = CreateUpdateDto(componentId: nonExistentId);

        // Act
        var result = await _sut.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Whiteboard not found"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that the service returns a failure result
    /// when validation fails due to an invalid markerColor.
    /// </summary>
    [Test]
    [Description("Application-003: Service returns failure when markerColor is invalid")]
    public async Task UpdateWhiteboardAsync_InvalidMarkerColor_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidComponentId))
            .ReturnsAsync(existingWhiteboard);

        var updateDto = CreateUpdateDto(markerColor: "");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Invalid marker color"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
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
            .Setup(r => r.GetByIdAsync(ValidComponentId))
            .ReturnsAsync(existingWhiteboard);
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace("Classroom", 3.0f, 5.0f, 5.0f));

        var updateDto = CreateUpdateDto(
            width: 2.0f, height: 1.5f, depth: 0.1f,
            x: 10.0f, y: 0.0f, z: 10.0f,
            orientation: "North", markerColor: "Blue");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position exceeds learning space boundaries"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service returns a failure result
    /// when the new position overlaps with another component in the same learning space.
    /// </summary>
    [Test]
    [Description("Application-005: Service returns failure when position overlaps with existing component")]
    public async Task UpdateWhiteboardAsync_PositionOverlaps_ReturnsFailure()
    {
        // Arrange
        var existingWhiteboard = CreateExistingWhiteboard();
        var otherComponent = new Whiteboard(
            "WB-002", ValidLearningSpaceId,
            2.0f, 1.5f, 0.1f,
            5.0f, 0.0f, 5.0f,
            "North", "Green");

        _mockWhiteboardRepository
            .Setup(r => r.GetByIdAsync(ValidComponentId))
            .ReturnsAsync(existingWhiteboard);
        _mockWhiteboardRepository
            .Setup(r => r.GetByLearningSpaceIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new List<LearningComponent> { otherComponent });
        _mockLearningSpaceReadRepository
            .Setup(r => r.GetByIdAsync(ValidLearningSpaceId))
            .ReturnsAsync(new LearningSpace("Classroom", 3.0f, 10.0f, 10.0f));

        var updateDto = CreateUpdateDto(
            width: 2.0f, height: 1.5f, depth: 0.1f,
            x: 5.0f, y: 0.0f, z: 5.0f,
            orientation: "North", markerColor: "Blue");

        // Act
        var result = await _sut.UpdateWhiteboardAsync(updateDto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Position overlaps with existing component"));
        });
        _mockWhiteboardRepository.Verify(r => r.UpdateAsync(It.IsAny<Whiteboard>()), Times.Never);
    }
}
