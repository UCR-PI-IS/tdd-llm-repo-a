using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
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

    private static readonly Guid ValidId = Guid.NewGuid();
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 15);

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
    [Description("Application-001: Verify successful person creation when no duplicate exists")]
    public async Task CreatePersonAsync_NoDuplicates_ReturnsSuccess()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(ValidEmail)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber)).ReturnsAsync(false);
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
    [Description("Application-002: Verify that creation fails when a person with the same email already exists")]
    public async Task CreatePersonAsync_DuplicateEmail_ReturnsFailureWithEmailError()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(ValidEmail)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber)).ReturnsAsync(false);

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
    [Description("Application-003: Verify that creation fails when a person with the same identity number already exists")]
    public async Task CreatePersonAsync_DuplicateIdentityNumber_ReturnsFailureWithIdentityError()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(ValidEmail)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber)).ReturnsAsync(true);

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
    [Description("Application-004: Verify that creation fails when both email and identity number already exist")]
    public async Task CreatePersonAsync_BothDuplicates_ReturnsFailureWithAlreadyExistsError()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(ValidEmail)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber)).ReturnsAsync(true);

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
    [Description("Application-005: Verify that the service handles repository exceptions gracefully")]
    public async Task CreatePersonAsync_RepositoryThrows_ReturnsFailureWithDatabaseError()
    {
        // Arrange
        var person = CreateValidPerson();
        _mockRepository.Setup(r => r.ExistsByEmailAsync(ValidEmail)).ReturnsAsync(false);
        _mockRepository.Setup(r => r.ExistsByIdentityNumberAsync(ValidIdentityNumber)).ReturnsAsync(false);
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
