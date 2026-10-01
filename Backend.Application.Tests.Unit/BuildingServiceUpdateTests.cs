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
    private const int ExistingBuildingId = 1;
    private const string ExistingName = "Old Building";
    private const string ExistingColor = "Red";
    private const float ExistingHeight = 10.0f;
    private const float ExistingLength = 20.0f;
    private const float ExistingWidth = 15.0f;

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
    /// Creates a Building entity with the given id, name, and color for test setup.
    /// </summary>
    private static Building CreateExistingBuilding(int id, string name, string color)
    {
        return new Building(id, name, color, ExistingHeight, ExistingLength, ExistingWidth, 0f, 0f, 0f);
    }

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds.
    /// The repository's UpdateAsync should be called once with the updated building.
    /// </summary>
    [Test]
    [Description("Application-001: Updating an existing building with valid data calls repository UpdateAsync")]
    public async Task UpdateBuildingAsync_ValidData_CallsRepositoryUpdate()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding(ExistingBuildingId, ExistingName, ExistingColor);
        var updateDto = new UpdateBuildingDto(
            "New Name", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ExistingBuildingId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .ReturnsAsync(existingBuilding);

        // Act
        await _sut.UpdateBuildingAsync(ExistingBuildingId, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws BuildingNotFoundException
    /// and the repository's UpdateAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-002: Updating a non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistingId = 999;
        var updateDto = new UpdateBuildingDto(
            "New Name", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

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
    /// Application-003: Verify that updating a building with invalid data propagates the
    /// domain validation ArgumentException and the repository's UpdateAsync is never called.
    /// </summary>
    [Test]
    [Description("Application-003: Updating a building with invalid data propagates ArgumentException")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesArgumentException()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding(ExistingBuildingId, ExistingName, ExistingColor);
        var invalidDto = new UpdateBuildingDto(
            "", "Blue", 12.5f, 25.0f, 18.0f, 100.0f, 5.0f, 200.0f);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ExistingBuildingId))
            .ReturnsAsync(existingBuilding);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(ExistingBuildingId, invalidDto);
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
