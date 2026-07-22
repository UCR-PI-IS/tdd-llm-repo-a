using NUnit.Framework;
using Moq;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Tests.Unit;

[TestFixture]
public class LearningComponentServiceTests
{
    private Mock<ILearningComponentRepository> mockRepository;
    private LearningComponentService service;
    private string learningSpaceId;
    private string invalidLearningSpaceId;

    [SetUp]
    public void SetUp()
    {
        mockRepository = new Mock<ILearningComponentRepository>();
        service = new LearningComponentService(mockRepository.Object);
        learningSpaceId = "ls-001";
        invalidLearningSpaceId = "";
    }

    [Test]
    [Description("Verify service returns list of components when learning space has components")]
    public async Task GetComponentsByLearningSpaceIdAsync_ExistingComponents_ReturnsList()
    {
        // Arrange
        var components = new List<LearningComponent>
        {
            new LearningComponent("c1", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "North"),
            new LearningComponent("c2", learningSpaceId, 1f, 1f, 1f, 0f, 0f, 0f, "South")
        };
        mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(components);

        // Act
        var result = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    [Description("Verify service returns empty list when learning space has no components")]
    public async Task GetComponentsByLearningSpaceIdAsync_NoComponents_ReturnsEmptyList()
    {
        // Arrange
        mockRepository.Setup(r => r.GetComponentsByLearningSpaceIdAsync(learningSpaceId))
            .ReturnsAsync(new List<LearningComponent>());

        // Act
        var result = await service.GetComponentsByLearningSpaceIdAsync(learningSpaceId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null or empty")]
    public void GetComponentsByLearningSpaceIdAsync_NullOrEmptyId_ThrowsArgumentException()
    {
        // Arrange
        string emptyId = "";

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.GetComponentsByLearningSpaceIdAsync(emptyId));
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }

    [Test]
    [Description("Verify service throws exception when learning space ID is null")]
    public void GetComponentsByLearningSpaceIdAsync_NullId_ThrowsArgumentException()
    {
        // Arrange
        string? nullId = null;

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.GetComponentsByLearningSpaceIdAsync(nullId!));
        Assert.That(ex.ParamName, Is.EqualTo("learningSpaceId"));
    }
}
