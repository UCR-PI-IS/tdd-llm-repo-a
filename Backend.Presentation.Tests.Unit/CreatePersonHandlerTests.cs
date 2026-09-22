using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonHandler.HandleAsync"/>.
/// Covers intents Presentation-008 through Presentation-010.
/// </summary>
[TestFixture]
public class CreatePersonHandlerTests
{
    private Mock<IPersonService> _mockService = null!;

    private static readonly Guid ValidId = Guid.NewGuid();
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidIdentityNumber = "123456789";
    private static readonly DateTime ValidBirthDate = new DateTime(1990, 1, 15);

    [SetUp]
    public void SetUp()
    {
        _mockService = new Mock<IPersonService>();
    }

    [TearDown]
    public void TearDown()
    {
        _mockService.VerifyAll();
    }

    private static CreatePersonDto CreateValidDto()
    {
        return new CreatePersonDto(ValidFirstName, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);
    }

    /// <summary>
    /// Presentation-008: Verify handler returns success response when person creation succeeds.
    /// </summary>
    [Test]
    [Description("Presentation-008: Verify handler returns success response when person creation succeeds")]
    public async Task HandleAsync_ValidDtoAndServiceSuccess_ReturnsOk()
    {
        // Arrange
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Success());
        var dto = CreateValidDto();

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Ok<CreatePersonResponse>>());
            var okResult = (Ok<CreatePersonResponse>)result.Result;
            Assert.That(okResult.Value.Success, Is.True);
        });
    }

    /// <summary>
    /// Presentation-009: Verify handler returns validation error response when DTO validation fails.
    /// </summary>
    [Test]
    [Description("Presentation-009: Verify handler returns validation error response when DTO validation fails")]
    public async Task HandleAsync_InvalidDto_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreatePersonDto(null!, ValidLastName, ValidEmail, ValidIdentityNumber, ValidBirthDate);

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.That(result.Result, Is.InstanceOf<BadRequest<ValidationErrorResponse>>());
    }

    /// <summary>
    /// Presentation-010: Verify handler returns conflict error when person already exists (duplicate email).
    /// </summary>
    [Test]
    [Description("Presentation-010: Verify handler returns conflict error when person already exists (duplicate email)")]
    public async Task HandleAsync_DuplicatePerson_ReturnsConflict()
    {
        // Arrange
        _mockService.Setup(s => s.CreatePersonAsync(It.IsAny<Person>()))
            .ReturnsAsync(CreatePersonResult.Failure("A person with this email already exists."));
        var dto = CreateValidDto();

        // Act
        var result = await CreatePersonHandler.HandleAsync(_mockService.Object, dto);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.InstanceOf<Conflict<ErrorResponse>>());
            var conflictResult = (Conflict<ErrorResponse>)result.Result;
            Assert.That(conflictResult.Value.Message, Does.Contain("already exists"));
        });
    }
}
