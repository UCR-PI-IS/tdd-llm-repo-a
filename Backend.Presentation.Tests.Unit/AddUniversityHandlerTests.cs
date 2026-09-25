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
    [Description("Presentation-001: Successful creation returns Ok with success message")]
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
            Assert.That(result.Result, Is.InstanceOf<Ok<AddUniversityResponse>>());
            var okResult = (Ok<AddUniversityResponse>)result.Result;
            Assert.That(okResult.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that a duplicate university registration returns 400 BadRequest
    /// with an error message containing "already exists".
    /// </summary>
    [Test]
    [Description("Presentation-002: Duplicate university returns BadRequest with error message")]
    public async Task HandleAsync_DuplicateUniversity_ReturnsBadRequestWithErrorMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(new ServiceResult<bool>
            {
                IsSuccess = false,
                ErrorMessage = "University with name 'UCR' already exists"
            });

        // Act
        var result = await _sut.HandleAsync(ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that an invalid property format returns 400 BadRequest
    /// with a validation error message containing "required".
    /// </summary>
    [Test]
    [Description("Presentation-003: Invalid property format returns BadRequest with validation message")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequestWithValidationMessage()
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync("", ValidCountry))
            .ReturnsAsync(new ServiceResult<bool>
            {
                IsSuccess = false,
                ErrorMessage = "Name is required"
            });

        // Act
        var result = await _sut.HandleAsync("", ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("required"));
        });
    }
}
