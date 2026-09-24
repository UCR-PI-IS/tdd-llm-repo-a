using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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
    private const string ExistingName = "UCR";

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
    /// </summary>
    [Test]
    [Description("Application-001: Service adds valid university successfully")]
    public async Task AddUniversityAsync_ValidUniversity_ReturnsSuccess()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ValidName))
            .ReturnsAsync(false);
        _mockRepository
            .Setup(r => r.AddAsync(It.Is<University>(u => u.Name == ValidName && u.Country == ValidCountry)))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sut.AddUniversityAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            _mockRepository.Verify(r => r.AddAsync(It.Is<University>(u => u.Name == ValidName && u.Country == ValidCountry)), Times.Once);
        });
    }

    /// <summary>
    /// Application-002: Verify that adding a university with an existing name returns failure.
    /// </summary>
    [Test]
    [Description("Application-002: Service returns failure when university name already exists")]
    public async Task AddUniversityAsync_DuplicateName_ReturnsFailure()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.ExistsByNameAsync(ExistingName))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.AddUniversityAsync(ExistingName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-003: Verify that adding a university with null or empty name returns validation failure.
    /// </summary>
    [TestCase("", Description = "Application-003: Empty name returns validation failure")]
    [TestCase(null, Description = "Application-003: Null name returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyName_ReturnsValidationFailure(string? invalidName)
    {
        // Act
        var result = await _sut.AddUniversityAsync(invalidName!, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Name"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }

    /// <summary>
    /// Application-004: Verify that adding a university with null or empty country returns validation failure.
    /// </summary>
    [TestCase("", Description = "Application-004: Empty country returns validation failure")]
    [TestCase(null, Description = "Application-004: Null country returns validation failure")]
    public async Task AddUniversityAsync_NullOrEmptyCountry_ReturnsValidationFailure(string? invalidCountry)
    {
        // Act
        var result = await _sut.AddUniversityAsync(ValidName, invalidCountry!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Country"));
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
        });
    }
}
