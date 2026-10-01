using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.UpdateBuildingAsync"/>.
/// Covers intents Application-001 through Application-003 for PQL-AE-001-002.
/// </summary>
[TestFixture]
public class BuildingServiceUpdateTests
{
    private Mock<IBuildingRepository> _mockRepository = null!;
    private BuildingService _sut = null!;

    // Valid test data
    private const int ValidId = 1;
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
        _mockRepository = new Mock<IBuildingRepository>();
        _sut = new BuildingService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    private static Building CreateExistingBuilding()
    {
        return new Building(
            ValidId, "Old Name", "Red",
            5.0f, 10.0f, 8.0f,
            0f, 0f, 0f);
    }

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds and persists changes.
    /// </summary>
    [Test]
    [Description("Application-001: Updating an existing building with valid data succeeds and persists changes")]
    public async Task UpdateBuildingAsync_ValidData_UpdatesAndPersists()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        var updateDto = new UpdateBuildingDto(
            "New Name", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        await _sut.UpdateBuildingAsync(ValidId, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-002: Updating a non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistentBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistentId = 999;
        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Building?)null);

        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        BuildingNotFoundException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(nonExistentId, updateDto);
        }
        catch (BuildingNotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected BuildingNotFoundException was not thrown");
            Assert.That(caughtException!.BuildingId, Is.EqualTo(nonExistentId));
        });
        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Building>()),
            Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates ArgumentException and does not persist.
    /// </summary>
    [Test]
    [Description("Application-003: Updating a building with invalid data propagates ArgumentException and does not persist")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesArgumentException()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidId))
            .ReturnsAsync(existingBuilding);

        var invalidDto = new UpdateBuildingDto(
            "", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(ValidId, invalidDto);
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
        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Building>()),
            Times.Never);
    }
}
