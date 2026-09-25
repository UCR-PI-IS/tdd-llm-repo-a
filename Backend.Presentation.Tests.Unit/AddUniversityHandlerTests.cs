using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
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
    /// Presentation-001: Verify that successful university creation returns 200 OK
    /// with a success message.
    /// </summary>
    [Test]
    [Description("Presentation-001: Handler returns 200 OK with success message when creation succeeds")]
    public async Task HandleAsync_CreationSucceeds_ReturnsOkWithSuccessMessage()
    {
        // Arrange
        var university = new University(ValidName, ValidCountry);
        var serviceResult = new ServiceResult<University>
        {
            IsSuccess = true,
            Data = university
        };

        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await AddUniversityHandler.HandleAsync(_mockService.Object, ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<AddUniversityResponse>>());
            var okResult = (Ok<AddUniversityResponse>)result.Result;
            Assert.That(okResult.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002: Verify that duplicate university registration returns 400 BadRequest
    /// with an error message indicating the university already exists.
    /// </summary>
    [Test]
    [Description("Presentation-002: Handler returns 400 BadRequest when university already exists")]
    public async Task HandleAsync_DuplicateUniversity_ReturnsBadRequest()
    {
        // Arrange
        var serviceResult = new ServiceResult<University>
        {
            IsSuccess = false,
            ErrorMessage = "University with name 'UCR' already exists"
        };

        _mockService
            .Setup(s => s.AddUniversityAsync(ValidName, ValidCountry))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await AddUniversityHandler.HandleAsync(_mockService.Object, ValidName, ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("already exists"));
        });
    }

    /// <summary>
    /// Presentation-003: Verify that invalid property format returns 400 BadRequest
    /// with a validation error message.
    /// </summary>
    [Test]
    [Description("Presentation-003: Handler returns 400 BadRequest when property format is invalid")]
    public async Task HandleAsync_InvalidPropertyFormat_ReturnsBadRequest()
    {
        // Arrange
        var serviceResult = new ServiceResult<University>
        {
            IsSuccess = false,
            ErrorMessage = "Name is required"
        };

        _mockService
            .Setup(s => s.AddUniversityAsync("", ValidCountry))
            .ReturnsAsync(serviceResult);

        // Act
        var result = await AddUniversityHandler.HandleAsync(_mockService.Object, "", ValidCountry);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain("required"));
        });
    }
}
