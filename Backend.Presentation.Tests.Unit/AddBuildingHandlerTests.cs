using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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
    private Mock<IBuildingCreateService> _mockService = null!;

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
        _mockService = new Mock<IBuildingCreateService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static AddBuildingDto CreateValidDto() =>
        new AddBuildingDto(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

    /// <summary>
    /// Presentation-001: Verify that a valid building creation request
    /// returns 201 Created with the building data.
    /// </summary>
    [Test]
    [Description("Presentation-001: Valid request returns 201 Created with building response")]
    public async Task HandleAsync_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var createdBuilding = new Building(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ReturnsAsync(createdBuilding);

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<Created<AddBuildingResponse>>());
    }

    /// <summary>
    /// Presentation-002: Verify that attempting to add a duplicate building
    /// returns 409 Conflict with an error message containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate building returns 409 Conflict")]
    public async Task HandleAsync_DuplicateBuilding_ReturnsConflict()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new DuplicateBuildingException(
                $"Building with name '{ValidName}' already exists"));

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<string>>());
            var conflictResult = (Conflict<string>)result.Result;
            Assert.That(conflictResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that attempting to add a building with invalid data
    /// returns 400 Bad Request with an error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid data returns 400 Bad Request")]
    public async Task HandleAsync_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddBuildingAsync(It.IsAny<Building>()))
            .ThrowsAsync(new ArgumentException("Name cannot be empty", "name"));

        var dto = CreateValidDto();

        // Act
        var result = await AddBuildingHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("cannot be empty"));
        });
    }
}
