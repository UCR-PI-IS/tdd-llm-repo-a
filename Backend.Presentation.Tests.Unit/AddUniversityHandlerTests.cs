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
    private AddUniversityHandler _sut = null!;

    // Valid test data
    private const string ValidName = "UCR";
    private const string ValidCountry = "Costa Rica";

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IUniversityService>();
        _sut = new AddUniversityHandler(_mockService.Object);
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
    [Description("Presentation-001: Handler returns Ok with success message when service succeeds")]
    public async Task HandleAsync_ServiceSucceeds_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _sut.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<Ok<AddUniversityResponse>>());
            var okResult = result.Result as Ok<AddUniversityResponse>;
            Assert.That(okResult!.Value!.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest with error message.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns BadRequest when university name already exists")]
    public async Task HandleAsync_DuplicateName_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(Result.Failure($"University with name '{ValidName}' already exists"));

        // Act
        var result = await _sut.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format returns 400 BadRequest with validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns BadRequest with validation error when property format is invalid")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequest()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("", ValidCountry))
            .ReturnsAsync(Result.Failure("Name is required"));

        // Act
        var result = await _sut.HandleAsync("", ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = result.Result as BadRequest<string>;
            Assert.That(badRequestResult!.Value, Does.Contain("required"));
        });
    }
}
