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
    private Mock<IUniversityRepository> _mockUniversityRepository = null!;
    private UniversityService _sut = null!;

    // Valid test data
    private const string ValidName = "Universidad de Costa Rica";
    private const string ValidCountry = "Costa Rica";

    [SetUp]
    public void SetUp()
    {
        _mockUniversityRepository = new Mock<IUniversityRepository>();
        _sut = new UniversityService(_mockUniversityRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockUniversityRepository.VerifyAll();
    }

    /// <summary>
    /// Application-001: Verify that a valid university is successfully added through the service.
    /// The service checks for duplicates, creates the entity, persists it, and returns success.
    /// </summary>
    [Test]
    [Description("Application-001: Valid university is added successfully and persisted")]
    public async Task AddUniversityAsync_ValidUniversity_ReturnsSuccessAndPersists()
    {
        // Arrange
        _mockUniversityRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockUniversityRepository
            .Setup(r => r.AddAsync(It.IsAny<University>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, ValidCountry);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _mockUniversityRepository.Verify(
            r => r.AddAsync(It.Is<University>(u =>
                u.Name == ValidName && u.Country == ValidCountry)),
            Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure
    /// with an "already exists" error message and does not persist the entity.
    /// </summary>
    [Test]
    [Description("Application-002: Duplicate university name returns failure with 'already exists' message")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailureWithAlreadyExistsMessage()
    {
        // Arrange
        var duplicateName = "UCR";
        _mockUniversityRepository
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
        _mockUniversityRepository.Verify(
            r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that adding a university with a null or empty name
    /// returns validation failure with a message containing "Name" and does not persist.
    /// </summary>
    [TestCase("", Description = "Application-003: Empty name returns validation failure")]
    [TestCase(null!, Description = "Application-003: Null name returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyName_ReturnsValidationFailure(string invalidName)
    {
        // Arrange — no repository setup needed; validation should short-circuit

        // Act
        var result = await _sut.AddUniversityAsync(invalidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name"));
        });
        _mockUniversityRepository.Verify(
            r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that adding a university with a null or empty country
    /// returns validation failure with a message containing "Country" and does not persist.
    /// </summary>
    [TestCase("", Description = "Application-004: Empty country returns validation failure")]
    [TestCase(null!, Description = "Application-004: Null country returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyCountry_ReturnsValidationFailure(string invalidCountry)
    {
        // Arrange — no repository setup needed; validation should short-circuit

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, invalidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
        });
        _mockUniversityRepository.Verify(
            r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }
}
