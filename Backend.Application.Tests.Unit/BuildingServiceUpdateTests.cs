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
    /// and calls <see cref="IBuildingRepository.UpdateAsync"/> exactly once.
    /// </summary>
    [Test]
    [Description("Application-001: Valid update of existing building succeeds and persists changes")]
    public async Task UpdateBuildingAsync_ExistingBuildingValidData_SucceedsAndCallsUpdateAsync()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, "Old Name", "Red",
            5.0f, 10.0f, 8.0f,
            0.0f, 0.0f, 0.0f);

        var updateDto = new UpdateBuildingDto(
            "New Name", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.UpdateBuildingAsync(ValidInternalId, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws
    /// <see cref="BuildingNotFoundException"/> containing the requested building id.
    /// </summary>
    [Test]
    [Description("Application-002: Update of non-existing building throws BuildingNotFoundException")]
    public async Task UpdateBuildingAsync_NonExistentBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        var nonExistentId = 999;
        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Building?)null);

        // Act
        var ex = await Assert.ThrowsAsync<BuildingNotFoundException>(() =>
            _sut.UpdateBuildingAsync(nonExistentId, updateDto));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.BuildingId, Is.EqualTo(nonExistentId));
        });
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates
    /// the domain validation exception and never calls <see cref="IBuildingRepository.UpdateAsync"/>.
    /// </summary>
    [Test]
    [Description("Application-003: Invalid update data propagates ArgumentException and never calls UpdateAsync")]
    public async Task UpdateBuildingAsync_InvalidData_PropagatesArgumentExceptionAndNeverCallsUpdateAsync()
    {
        // Arrange
        var existingBuilding = new Building(
            ValidInternalId, ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        var invalidDto = new UpdateBuildingDto(
            "", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        _mockRepository
            .Setup(r => r.GetByIdAsync(ValidInternalId))
            .ReturnsAsync(existingBuilding);

        // Act
        var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.UpdateBuildingAsync(ValidInternalId, invalidDto));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.ParamName, Is.EqualTo("name"));
        });
        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Building>()),
            Times.Never);
    }
}
