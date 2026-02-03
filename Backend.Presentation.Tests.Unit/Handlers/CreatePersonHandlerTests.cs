using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Requests;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.Handlers;

/// <summary>
/// Unit tests for the CreatePersonHandler.
/// Tests cover HTTP request handling for person creation endpoint.
/// </summary>
[TestFixture]
public class CreatePersonHandlerTests
{
    private Mock<IPersonService> _mockService;
    private CreatePersonHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonService>();
        _handler = new CreatePersonHandler(_mockService.Object);
    }

    [Test(Description = "HandleAsync should return 200 OK with success response when person is created successfully")]
    public async Task HandleAsync_WithValidRequest_ShouldReturn200OK()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IdentityNumber = "123456789",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "555-1234"
        };

        _mockService
            .Setup(s => s.CreatePersonAsync(
                "John",
                "Doe",
                "john.doe@example.com",
                "123456789",
                It.IsAny<DateTime>(),
                "555-1234"))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(200));
        Assert.That(result.Success, Is.True);
        _mockService.Verify(s => s.CreatePersonAsync(
            "John",
            "Doe",
            "john.doe@example.com",
            "123456789",
            It.IsAny<DateTime>(),
            "555-1234"), Times.Once);
    }

    [Test(Description = "HandleAsync should return 400 Bad Request when person already exists (duplicate email)")]
    public async Task HandleAsync_WithDuplicateEmail_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "duplicate@example.com",
            IdentityNumber = "123456789",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "555-1234"
        };

        _mockService
            .Setup(s => s.CreatePersonAsync(
                "John",
                "Doe",
                "duplicate@example.com",
                "123456789",
                It.IsAny<DateTime>(),
                "555-1234"))
            .ThrowsAsync(new InvalidOperationException("Person with this email already exists"));

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already exists"));
    }

    [Test(Description = "HandleAsync should return 400 Bad Request when required fields are missing")]
    public async Task HandleAsync_WithMissingRequiredFields_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = null, // Missing required field
            LastName = "Doe",
            Email = "john.doe@example.com",
            IdentityNumber = "123456789",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "555-1234"
        };

        _mockService
            .Setup(s => s.CreatePersonAsync(
                null,
                "Doe",
                "john.doe@example.com",
                "123456789",
                It.IsAny<DateTime>(),
                "555-1234"))
            .ThrowsAsync(new ArgumentException("FirstName is required"));

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("required"));
    }

    [Test(Description = "HandleAsync should return 400 Bad Request when duplicate identity number exists")]
    public async Task HandleAsync_WithDuplicateIdentityNumber_ShouldReturn400BadRequest()
    {
        // Arrange
        var request = new CreatePersonRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            IdentityNumber = "999999999",
            BirthDate = new DateTime(1990, 1, 1),
            Phone = "555-1234"
        };

        _mockService
            .Setup(s => s.CreatePersonAsync(
                "John",
                "Doe",
                "john.doe@example.com",
                "999999999",
                It.IsAny<DateTime>(),
                "555-1234"))
            .ThrowsAsync(new InvalidOperationException("Person with this identity number already exists"));

        // Act
        var result = await _handler.HandleAsync(request);

        // Assert
        Assert.That(result.StatusCode, Is.EqualTo(400));
        Assert.That(result.Success, Is.False);
        Assert.That(result.Message, Does.Contain("already exists"));
    }
}
