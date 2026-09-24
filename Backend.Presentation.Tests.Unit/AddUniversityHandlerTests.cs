using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AddUniversityHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003.
/// </summary>
[TestFixture]
public class AddUniversityHandlerTests
{
    private Mock<IUniversityService> _mockService = null!;

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IUniversityService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    /// <summary>
    /// Presentation-001: Verify that successful university creation returns 200 OK
    /// with a success message in the response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Successful university creation returns 200 OK with success message")]
    public async Task HandleAsync_SuccessfulCreation_ReturnsOk()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("UCR", "Costa Rica"))
            .ReturnsAsync(Result.Success());

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync("UCR", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<Ok<AddUniversityResponse>>());
            var okResult = (Ok<AddUniversityResponse>)result.Result;
            Assert.That(okResult.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest
    /// with an error message indicating the university already exists.
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate university registration returns 400 BadRequest")]
    public async Task HandleAsync_DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("UCR", "Costa Rica"))
            .ReturnsAsync(Result.Failure("University with name 'UCR' already exists"));

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync("UCR", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format (empty name) returns 400 BadRequest
    /// with a validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid property format returns 400 BadRequest with validation error")]
    public async Task HandleAsync_InvalidName_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("", "Costa Rica"))
            .ReturnsAsync(Result.Failure("Name is required"));

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync("", "Costa Rica");

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("required"));
        });
    }
}
