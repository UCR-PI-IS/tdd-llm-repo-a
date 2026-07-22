using NUnit.Framework;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

[TestFixture]
public class GetLearningComponentsHandlerTests
{
    private Mock<ILearningComponentService> mockService;
    private string learningSpaceId;
    private string invalidLearningSpaceId;
    private string nonExistentLearningSpaceId;

    [SetUp]
    public void SetUp()
    {
        mockService = new Mock<ILearningComponentService>();
        learningSpaceId = "ls-001";
        invalidLearningSpaceId = "";
        nonExistentLearningSpaceId = "ls-999";
    }

    [Test]
    [Description("Verify handler returns OK response with list of components when learning space has components")]
    public async Task HandleAsync_ComponentsExist_ReturnsOkWithList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("c1", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "North"),
            new LearningComponent("c2", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "South")
        };
        mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
    }

    [Test]
    [Description("Verify handler returns OK response with empty list when learning space has no components")]
    public async Task HandleAsync_NoComponents_ReturnsOkWithEmpty()
    {
        // Arrange
        mockService.Setup(s => s.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(mockService.Object, learningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<Ok<GetLearningComponentsResponse>>());
    }

    [Test]
    [Description("Verify handler returns BadRequest response when learning space ID is null or empty")]
    public async Task HandleAsync_InvalidId_ReturnsBadRequest()
    {
        // Arrange

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(mockService.Object, invalidLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<BadRequest<ErrorResponse>>());
    }

    [Test]
    [Description("Verify handler returns NotFound response when learning space does not exist")]
    public async Task HandleAsync_NonExistentLearningSpace_ReturnsNotFound()
    {
        // Arrange

        // Act
        var result = await GetLearningComponentsHandler.HandleAsync(mockService.Object, nonExistentLearningSpaceId);

        // Assert
        Assert.That(result, Is.TypeOf<NotFound<ErrorResponse>>());
    }
}
