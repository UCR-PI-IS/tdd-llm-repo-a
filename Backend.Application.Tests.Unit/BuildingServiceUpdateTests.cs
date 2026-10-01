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
            1, "Old Name", "Red",
            10.0f, 20.0f, 15.0f,
            0f, 0f, 0f);
    }

    /// <summary>
    /// Application-001: Verify that updating an existing building with valid data
    /// succeeds and invokes the repository update.
    /// </summary>
    [Test]
    [Description("Application-001: Updating an existing building with valid data succeeds")]
    public async Task UpdateBuildingAsync_ExistingBuildingWithValidData_Succeeds()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingBuilding);
        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Building>()))
            .Returns(Task.CompletedTask);

        var updateDto = new UpdateBuildingDto(
            "New Name", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        await _sut.UpdateBuildingAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(
            r => r.UpdateAsync(It.Is<Building>(b => b.Name == "New Name")),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that updating a non-existing building throws
    /// <see cref="BuildingNotFoundException"/> and never calls UpdateAsync.
    /// </summary>
    [Test]
    [Description("Application-002: Updating a non-existing building throws BuildingNotFoundException")]
    public void UpdateBuildingAsync_NonExistingBuilding_ThrowsBuildingNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Building?)null);

        var updateDto = new UpdateBuildingDto(
            ValidName, ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        var ex = Assert.ThrowsAsync<BuildingNotFoundException>(
            () => _sut.UpdateBuildingAsync(999, updateDto));

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(ex, Is.Not.Null);
            Assert.That(ex!.BuildingId, Is.EqualTo(999));
        });
        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Building>()),
            Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that updating a building with invalid data propagates
    /// the domain validation exception and never calls UpdateAsync.
    /// </summary>
    [Test]
    [Description("Application-003: Updating a building with invalid data propagates ArgumentException")]
    public void UpdateBuildingAsync_InvalidData_ThrowsArgumentException()
    {
        // Arrange
        var existingBuilding = CreateExistingBuilding();
        _mockRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingBuilding);

        var invalidDto = new UpdateBuildingDto(
            "", ValidColor,
            ValidHeight, ValidLength, ValidWidth,
            ValidX, ValidY, ValidZ);

        // Act
        var ex = Assert.ThrowsAsync<ArgumentException>(
            () => _sut.UpdateBuildingAsync(1, invalidDto));

        // Assert
        Assert.That(ex!.ParamName, Is.EqualTo("name"));
        _mockRepository.Verify(
            r => r.UpdateAsync(It.IsAny<Building>()),
            Times.Never);
    }
}
