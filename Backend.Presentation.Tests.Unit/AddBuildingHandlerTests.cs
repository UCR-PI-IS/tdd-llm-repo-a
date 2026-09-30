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
/// Covers intents Presentation-001 through Presentation-003 for the Add Building story.
/// </summary>
[TestFixture]
public class AddBuildingHandlerTests
{
    private Mock<IBuildingService> _mockService = null!;

    // Valid test data
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Red";
    private const float ValidHeight = 20.5f;
    private const float ValidLength = 50.0f;
    private const float ValidWidth = 30.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 200.0f;
    private const float ValidZ = 0.0f;
    private const int ValidAreaId = 1;

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
    /// Presentation-001: Verify that a valid building creation request returns 201 Created
    /// with the building data in the response body.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid building creation returns 201 Created with building data")]
    public async Task HandleAsync_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new AddBuildingRequest(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        var building = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ReturnsAsync(building);

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
    /// Presentation-002: Verify that attempting to add a duplicate building returns 409 Conflict
    /// with an error message containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate building returns 409 Conflict with error message")]
    public async Task HandleAsync_DuplicateBuilding_ReturnsConflict()
    {
        // Arrange
        var request = new AddBuildingRequest(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new DuplicateBuildingException(
                $"Building with name '{ValidName}' already exists"));

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
    /// Presentation-003: Verify that attempting to add a building with invalid data returns
    /// 400 Bad Request with an error message containing "cannot be empty".
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid building data returns 400 Bad Request with validation message")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddBuildingRequest(
            "", ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

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
