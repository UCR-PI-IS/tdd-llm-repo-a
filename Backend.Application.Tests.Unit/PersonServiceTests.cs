using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="PersonService.CreatePersonAsync"/>.
/// Covers intents Application-001 through Application-005.
/// </summary>
[TestFixture]
public class PersonServiceTests
{
    private Mock<IPersonRepository> _mockRepository = null!;
    private PersonService _sut = null!;

    // Valid test data
    private const string ValidId = "PER-001";
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 1);

    [SetUp]
    public void SetUp()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _sut = new PersonService(_mockRepository.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockRepository.VerifyAll();
    }

    private static Person CreateValidPerson()
    {
        return new Person(ValidId, ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
    }

    /// <summary>
    /// Application-001: Verify successful person creation when no duplicate exists.
    /// </summary>
    [Test]
    [Description("Application-001: CreatePersonAsync succeeds when no duplicate exists")]
    public async Task CreatePersonAsync_NoDuplicate_ReturnsSuccess()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(person.Email)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(person.IdentityNumber)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(person)).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreatePersonAsync(person);

        // Assert
        Assert.That(result.IsSuccess, Is.True);
        _mockRepository.Verify(r => r.AddAsync(person), Times.Once);
    }

    /// <summary>
    /// Application-002: Verify that creation fails when a person with the same email already exists.
    /// </summary>
    [Test]
    [Description("Application-002: CreatePersonAsync fails when duplicate email exists")]
    public async Task CreatePersonAsync_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(person.Email)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this email already exists."));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-003: Verify that creation fails when a person with the same identity number already exists.
    /// </summary>
    [Test]
    [Description("Application-003: CreatePersonAsync fails when duplicate identity number exists")]
    public async Task CreatePersonAsync_DuplicateIdentityNumber_ReturnsFailure()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(person.Email)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(person.IdentityNumber)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("A person with this identity number already exists."));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-004: Verify that creation fails when both email and identity number already exist.
    /// </summary>
    [Test]
    [Description("Application-004: CreatePersonAsync fails when both email and identity number are duplicates")]
    public async Task CreatePersonAsync_BothDuplicates_ReturnsFailure()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(person.Email)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(person.IdentityNumber)).ReturnsAsync(true);

        // Act
        var result = await _sut.CreatePersonAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("already exists"));
        });
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Never);
    }

    /// <summary>
    /// Application-005: Verify that the service handles repository exceptions gracefully.
    /// </summary>
    [Test]
    [Description("Application-005: CreatePersonAsync handles repository exceptions gracefully")]
    public async Task CreatePersonAsync_RepositoryException_ReturnsFailure()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(person.Email)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(person.IdentityNumber)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.AddAsync(person)).ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _sut.CreatePersonAsync(person);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("Database connection failed"));
        });
    }
}
