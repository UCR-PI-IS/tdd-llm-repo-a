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
    /// Presentation-001: Verify that successful university creation returns 200 OK
    /// with the success message in the response.
    /// </summary>
    [Test]
    [Description("Presentation-001: Successful university creation returns 200 OK with success message")]
    public async Task HandleAsync_ServiceReturnsSuccess_ReturnsOkWithResponse()
    {
        // Arrange
        var name = "UCR";
        var country = "Costa Rica";
        _mockService
            .Setup(s => s.AddUniversityAsync(name, country))
            .ReturnsAsync(new ServiceResult<bool> { IsSuccess = true });

        // Act
        var result = await _sut.HandleAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<AddUniversityResponse>>());
            var okResult = (Ok<AddUniversityResponse>)result.Result;
            Assert.That(okResult.Value.Message, Is.EqualTo("University added successfully"));
        });
    }

    /// <summary>
    /// Presentation-002 and Presentation-003: Verify that when the service returns a failure,
    /// the handler returns 400 BadRequest with the appropriate error message.
    /// </summary>
    [TestCase("UCR", "Costa Rica", "University with name 'UCR' already exists", "already exists",
        Description = "Presentation-002: Duplicate university registration returns 400 BadRequest")]
    [TestCase("", "Costa Rica", "Name is required", "required",
        Description = "Presentation-003: Invalid property format returns 400 BadRequest with validation error")]
    public async Task HandleAsync_ServiceReturnsFailure_ReturnsBadRequest(
        string name, string country, string errorMessage, string expectedMessageContent)
    {
        // Arrange
        _mockService
            .Setup(s => s.AddUniversityAsync(name, country))
            .ReturnsAsync(new ServiceResult<bool> { IsSuccess = false, ErrorMessage = errorMessage });

        // Act
        var result = await _sut.HandleAsync(name, country);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<BadRequest<string>>());
            var badRequestResult = (BadRequest<string>)result.Result;
            Assert.That(badRequestResult.Value, Does.Contain(expectedMessageContent));
        });
    }
}
