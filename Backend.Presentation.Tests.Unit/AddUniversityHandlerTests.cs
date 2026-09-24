using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="AddUniversityHandler.HandleAsync"/>.
/// Covers intents Presentation-001 through Presentation-003.
/// </summary>
[TestFixture]
public class AddUniversityHandlerTests
{
    private Mock<IUniversityService> _mockService = null!;

    // Valid test data
    private const string ValidName = "UCR";
    private const string ValidCountry = "Costa Rica";

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
    /// Presentation-001: Verify that successful university creation returns 200 OK with success message.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK when university creation succeeds")]
    public async Task HandleAsync_CreationSucceeds_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var serviceResult = ServiceResult.Success();
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(serviceResult);

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.That(result.Result, Is.TypeOf<Ok<AddUniversityResponse>>());
        var okResult = result.Result as Ok<AddUniversityResponse>;
        Assert.That(okResult!.Value.Message, Is.EqualTo("University added successfully"));
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 BadRequest when university already exists")]
    public async Task HandleAsync_DuplicateUniversity_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        var serviceResult = ServiceResult.Failure("University with name 'UCR' already exists");
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(serviceResult);

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
        var badRequestResult = result.Result as BadRequest<string>;
        Assert.That(badRequestResult!.Value, Does.Contain("already exists"));
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format returns 400 BadRequest with validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 BadRequest when validation fails")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequestWithValidationError()
    {
        // Arrange
        var serviceResult = ServiceResult.Failure("Name is required");
        _mockService
            .Setup(s => s.AddUniversityAsync("", ValidCountry))
            .ReturnsAsync(serviceResult);

        var handler = new AddUniversityHandler(_mockService.Object);

        // Act
        var result = await handler.HandleAsync("", ValidCountry);

        // Assert
        Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
        var badRequestResult = result.Result as BadRequest<string>;
        Assert.That(badRequestResult!.Value, Does.Contain("required"));
    }
}
