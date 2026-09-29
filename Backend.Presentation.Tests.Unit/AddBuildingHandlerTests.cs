using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AddBuildingHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003.
/// </summary>
[TestFixture]
public class AddBuildingHandlerTests
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
    /// Presentation-001: Verify that a valid building creation request returns 201 Created with the building data.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid request returns 201 Created with building data")]
    public async Task HandleAsync_ValidRequest_ReturnsCreatedWithBuildingData()
    {
        // Arrange
        var request = new CreateBuildingDto(
            "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        var createdBuilding = new Building(
            "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ReturnsAsync(createdBuilding);

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Created<AddBuildingResponse>>());
            var createdResult = (Created<AddBuildingResponse>)result;
            Assert.That(createdResult.Value.Building.Name, Is.EqualTo(request.Name));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that attempting to add a duplicate building returns 409 Conflict with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate building returns 409 Conflict")]
    public async Task HandleAsync_DuplicateBuilding_ReturnsConflictWithErrorMessage()
    {
        // Arrange
        var request = new CreateBuildingDto(
            "Engineering Building", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new DuplicateBuildingException("Building with name 'Engineering Building' already exists"));

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<Conflict<string>>());
            var conflictResult = (Conflict<string>)result;
            Assert.That(conflictResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that attempting to add a building with invalid data returns 400 Bad Request with error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid data returns 400 Bad Request")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        var request = new CreateBuildingDto(
            "", "Red", 20.5f, 50.0f, 30.0f, 100.0f, 200.0f, 0.0f, 1);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty"));

        // Act
        var result = await AddBuildingHandler.HandleAsync(request, _mockService.Object);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result;
            Assert.That(badRequestResult.Value, Does.Contain("cannot be empty"));
        });
    }
}
