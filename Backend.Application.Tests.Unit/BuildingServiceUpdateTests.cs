using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="BuildingService.UpdateBuildingAsync"/>.
/// Covers intents Application-001 through Application-003 for story PQL-AE-001-002.
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
    /// The service should retrieve the building, apply updates, and persist changes.
    /// </summary>
    [Test]
    [Description("Application-001: Updating existing building with valid data succeeds")]
    public async Task UpdateBuildingAsync_ExistingBuildingValidData_UpdatesSuccessfully()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, "Old Name", "Red", 5.0f, 10.0f, 8.0f, 50.0f, 0.0f, 100.0f);

        var updatedBuilding = new Building(
            ValidInternalId, "New Name", "Blue", 12.0f, 25.0f, 18.0f, 150.0f, 5.0f, 250.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(ValidInternalId, updatedBuilding);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException.
    /// </summary>
    [Test]
    [Description("Application-002: Updating non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        const int nonExistentId = 999;
        var building = new Building(
            nonExistentId, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ, ValidAreaId);

        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Building?)null);

        // Act
        BuildingNotFoundException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(nonExistentId, building);
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
    [Description("Application-003: Updating building with invalid data propagates validation exception")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesValidationException()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, "Old Name", "Red", 5.0f, 10.0f, 8.0f, 50.0f, 0.0f, 100.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);

        // Act - Try to update with empty name (should trigger validation)
        ArgumentException? caughtException = null;
        try
        {
            var invalidBuilding = new Building(
                ValidInternalId, "", ValidColor, ValidHeight, ValidLength, ValidWidth,
                ValidX, ValidY, ValidZ, ValidAreaId);
            await _sut.UpdateBuildingAsync(ValidInternalId, invalidBuilding);
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
