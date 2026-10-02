using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.UpdateBuildingAsync"/>.
/// Covers intents Application-001 through Application-003 for the Edit Building story (PQL-AE-001-002).
/// </summary>
[TestFixture]
public class BuildingServiceUpdateTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
    private BuildingService _sut = null!;

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

    private Building _existingBuilding = null!;

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IBuildingRepository>();
        _sut = new BuildingService(_mockRepository.Object);
        _existingBuilding = new Building(
            ValidBuildingId, "Old Name", "Red",
            10.0f, 20.0f, 15.0f,
            0f, 0f, 0f);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds.
    /// The service should retrieve the building, apply the update, and persist the changes.
    /// </summary>
    [Test]
    [Description("Application-001: UpdateBuildingAsync with valid data updates building and persists changes")]
    public async Task UpdateBuildingAsync_ValidData_UpdatesBuildingSuccessfully()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            "New Name", ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidBuildingId))
            .ReturnsAsync(_existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(ValidBuildingId, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException.
    /// The service should not attempt to persist any changes.
    /// </summary>
    [Test]
    [Description("Application-002: UpdateBuildingAsync with non-existing building ID throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistingId = 999;
        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistingId))
            .ReturnsAsync((Building?)null);

        // Act
        BuildingNotFoundException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(nonExistingId, updateDto);
        }
        catch (BuildingNotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected BuildingNotFoundException was not thrown");
            Assert.That(caughtException!.BuildingId, Is.EqualTo(nonExistingId));
        });
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates the domain
    /// validation exception (ArgumentException) and does not persist any changes.
    /// </summary>
    [Test]
    [Description("Application-003: UpdateBuildingAsync with invalid data propagates ArgumentException and does not persist")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesArgumentException()
    {
        // Arrange
        var invalidDto = new UpdateBuildingDto(
            "", ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidBuildingId))
            .ReturnsAsync(_existingBuilding);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(ValidBuildingId, invalidDto);
        }
        catch (ArgumentException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected ArgumentException was not thrown");
            Assert.That(caughtException!.ParamName, Is.EqualTo("name"));
        });
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Building>()), Times.Never);
    }
}
