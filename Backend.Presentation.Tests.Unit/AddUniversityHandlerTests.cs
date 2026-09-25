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
    /// Presentation-001: Verify that a successful university creation returns 200 OK
    /// with an AddUniversityResponse containing the success message.
    /// </summary>
    [Test]
    [Description("Presentation-001: Successful university creation returns 200 OK with success message")]
    public async Task HandleAsync_SuccessfulCreation_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(new ServiceResult<bool> { IsSuccess = true });

        // Act
        var result = await _sut.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<Ok<AddUniversityResponse>>());
            var okResult = (Ok<AddUniversityResponse>)result.Result;
            Assert.That(okResult.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that a duplicate university registration returns 400 BadRequest
    /// with an error message indicating the university already exists.
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate university registration returns 400 BadRequest")]
    public async Task HandleAsync_DuplicateUniversity_ReturnsBadRequest()
    {
        // Arrange
        var errorMessage = "University with name 'UCR' already exists";
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(new ServiceResult<bool> { IsSuccess = false, ErrorMessage = errorMessage });

        // Act
        var result = await _sut.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that an invalid property format returns 400 BadRequest
    /// with a validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid property format returns 400 BadRequest with validation error")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequest()
    {
        // Arrange
        var emptyName = "";
        var errorMessage = "Name is required";
        _mockService
            .Setup(s => s.AddUniversityAsync(emptyName, ValidCountry))
            .ReturnsAsync(new ServiceResult<bool> { IsSuccess = false, ErrorMessage = errorMessage });

        // Act
        var result = await _sut.HandleAsync(emptyName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.TypeOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("required"));
        });
    }
}
