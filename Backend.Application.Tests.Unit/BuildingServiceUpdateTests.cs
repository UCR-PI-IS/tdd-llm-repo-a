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

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data succeeds
    /// and triggers repository update exactly once.
    /// </summary>
    [Test]
    [Description("Application-001: Updating an existing building with valid data succeeds and calls UpdateAsync once")]
    public async Task UpdateBuildingAsync_ValidData_ReturnsUpdatedBuilding()
    {
        // Arrange
        var existingBuilding = new Building(
            1, "Old Name", "Red", 5.0f, 10.0f, 8.0f, 0.0f, 0.0f, 0.0f);

        var updateDto = new UpdateBuildingDto(
            "New Name", ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws
    /// BuildingNotFoundException with the correct BuildingId.
    /// </summary>
    [Test]
    [Description("Application-002: Updating a non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistentBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Building?)null);

        // Act
        BuildingNotFoundException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(999, updateDto);
        }
        catch (BuildingNotFoundException ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(caughtException, Is.Not.Null, "Expected BuildingNotFoundException was not thrown");
            Assert.That(caughtException!.BuildingId, Is.EqualTo(999));
        });
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates the
    /// domain validation exception and never calls UpdateAsync.
    /// </summary>
    [Test]
    [Description("Application-003: Updating a building with invalid data propagates ArgumentException and does not call UpdateAsync")]
    public async Task UpdateBuildingAsync_InvalidData_ThrowsArgumentExceptionAndDoesNotUpdate()
    {
        // Arrange
        var existingBuilding = new Building(
            1, ValidName, ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);

        var invalidDto = new UpdateBuildingDto(
            "", ValidColor, ValidHeight, ValidLength, ValidWidth, ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingBuilding);

        // Act
        ArgumentException? caughtException = null;
        try
        {
            await _sut.UpdateBuildingAsync(1, invalidDto);
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
