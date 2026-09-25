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
    /// The repository confirms the name does not exist, the university is persisted, and the result indicates success.
    /// </summary>
    [Test]
    [Description("Application-001: Valid university is successfully added and persisted")]
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
    /// Application-003 and Application-004: Verify that adding a university with a null or empty
    /// property returns validation failure and does not persist the entity.
    /// </summary>
    [TestCase("", ValidCountry, "Name",
        Description = "Application-003: Empty name returns validation failure")]
    [TestCase(ValidName, "", "Country",
        Description = "Application-004: Empty country returns validation failure")]
    public async Task AddUniversityAsync_InvalidProperty_ReturnsValidationFailureAndDoesNotPersist(
        string name, string country, string expectedMessageSubstring)
    {
        // Arrange — no repository setup needed; validation occurs before repository calls

        // Act
        var result = await _sut.AddUniversityAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain(expectedMessageSubstring));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<University>()), Times.Never);
    }
}
