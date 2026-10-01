using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003 for story PQL-AE-001-002.
/// </summary>
[TestFixture]
public class UpdateBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

    // Valid test data
    private const int ValidBuildingId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IBuildingService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify that updating a building with valid data returns 200 OK with success response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid update returns 200 OK with updated building data")]
    public async Task HandleAsync_ValidRequest_ReturnsOkWithUpdatedData()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            ValidBuildingId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidBuildingId, It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidBuildingId, updateDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Ok<UpdateBuildingResponse>>());
    }

    /// <summary>
    /// Presentation-002: Verify that updating a building with invalid data returns 400 Bad Request with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Invalid data returns 400 Bad Request with error message")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidDto = new UpdateBuildingDto(
            ValidBuildingId, "", ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(ValidBuildingId, It.IsAny<Building>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(ValidBuildingId, invalidDto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("Name cannot be empty"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that updating a non-existing building returns 404 Not Found with error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Non-existing building returns 404 Not Found")]
    public async Task HandleAsync_NonExistingBuilding_ReturnsNotFound()
    {
        // Arrange
        const int nonExistentId = 999;
        var updateDto = new UpdateBuildingDto(
            nonExistentId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(nonExistentId, It.IsAny<Building>()))
            .ThrowsAsync(new BuildingNotFoundException(nonExistentId));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(nonExistentId, updateDto, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
            var notFoundResult = (NotFound<string>)result.Result;
            Assert.That(notFoundResult.Value, Does.Contain("not found"));
        });
    }
}
