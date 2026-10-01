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
    private const int ValidInternalId = 1;
    private const string ValidName = "Engineering Building";
    private const string ValidColor = "Blue";
    private const float ValidHeight = 10.5f;
    private const float ValidLength = 20.0f;
    private const float ValidWidth = 15.0f;
    private const float ValidX = 100.0f;
    private const float ValidY = 0.0f;
    private const float ValidZ = 200.0f;
    private const int ValidAreaId = 1;

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

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds.
    /// The service should retrieve the building, update it, and persist changes.
    /// </summary>
    [Test]
    [Description("Application-001: Update existing building with valid data succeeds")]
    public async Task UpdateBuildingAsync_ExistingBuildingValidData_UpdatesSuccessfully()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0f, 0f, 0f);

        var updateDto = new UpdateBuildingDto(
            "New Name", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(ValidInternalId, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Building>(b => 
            b.Name == "New Name" && 
            b.Color == "Blue" &&
            b.Height == 12.5f)), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-002: Update non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistentId = 999;
        var updateDto = new UpdateBuildingDto(
            "New Name", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Building?)null);

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
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Building>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates the domain validation exception.
    /// The repository UpdateAsync should never be called when validation fails.
    /// </summary>
    [Test]
    [Description("Application-003: Update with invalid data propagates ArgumentException")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesArgumentException()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, "Old Name", "Red", 10.0f, 20.0f, 15.0f, 0f, 0f, 0f);

        var invalidDto = new UpdateBuildingDto(
            "", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(ValidInternalId, invalidDto);
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
