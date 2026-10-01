using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UpdateBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003 for PQL-AE-001-002.
/// </summary>
[TestFixture]
public class UpdateBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

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
    /// Presentation-001: Verify that updating a building with valid data returns 200 OK
    /// with a success response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Updating a building with valid data returns 200 OK")]
    public async Task HandleAsync_ValidRequest_ReturnsOkWithResponse()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            "Updated Building", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(1, It.IsAny<UpdateBuildingDto>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(1, updateDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Ok<UpdateBuildingResponse>>());
    }

    /// <summary>
    /// Presentation-002: Verify that updating a building with invalid data returns 400 Bad Request.
    /// </summary>
    [Test]
    [Description("Presentation-002: Updating a building with invalid data returns 400 Bad Request")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidDto = new UpdateBuildingDto(
            "", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(1, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(1, invalidDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
    }

    /// <summary>
    /// Presentation-003: Verify that updating a non-existing building returns 404 Not Found.
    /// </summary>
    [Test]
    [Description("Presentation-003: Updating a non-existing building returns 404 Not Found")]
    public async Task HandleAsync_NonExistingBuilding_ReturnsNotFound()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            "Updated Building", "Blue",
            12.5f, 25.0f, 18.0f,
            100.0f, 5.0f, 200.0f);

        _mockService
            .Setup(s => s.UpdateBuildingAsync(999, It.IsAny<UpdateBuildingDto>()))
            .ThrowsAsync(new BuildingNotFoundException(999));

        // Act
        var result = await UpdateBuildingHandler.HandleAsync(999, updateDto, _mockService.Object);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<NotFound<string>>());
    }
}
