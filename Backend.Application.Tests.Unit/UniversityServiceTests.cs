using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="UniversityService.AddUniversityAsync"/>.
/// Covers intents Application-001 through Application-004.
/// </summary>
[TestFixture]
public class UniversityServiceTests
{
    private Mock<IUniversityRepository> _mockRepository = null!;
    private UniversityService _sut = null!;

    // Valid test data
    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IUniversityRepository>();
        _sut = new UniversityService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid university is successfully added through the service.
    /// The repository confirms the name does not exist, and the service persists the new entity.
    /// </summary>
    [Test]
    [Description("Application-001: Valid university is added successfully and persisted")]
    public async Task AddUniversityAsync_ValidUniversity_ReturnsSuccessAndPersists()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<University>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, ValidCountry);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _mockRepository.Verify(
            r => r.AddAsync(It.Is<University>(u => u.Name == ValidName && u.Country == ValidCountry)),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure
    /// and does not persist the entity.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate university name returns failure and does not persist")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailureAndDoesNotPersist()
    {
        // Arrange
        var duplicateName = "UCR";
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(duplicateName))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.AddUniversityAsync(duplicateName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a university with a null or empty name
    /// returns validation failure and does not persist the entity.
    /// </summary>
    [Test]
    [Description("Application-003: Empty name returns validation failure and does not persist")]
    public async Task AddUniversityAsync_EmptyName_ReturnsValidationFailure()
    {
        // Arrange
        var emptyName = "";

        // Act
        var result = await _sut.AddUniversityAsync(emptyName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that adding a university with a null or empty country
    /// returns validation failure and does not persist the entity.
    /// </summary>
    [Test]
    [Description("Application-004: Empty country returns validation failure and does not persist")]
    public async Task AddUniversityAsync_EmptyCountry_ReturnsValidationFailure()
    {
        // Arrange
        var emptyCountry = "";

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, emptyCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }
}
