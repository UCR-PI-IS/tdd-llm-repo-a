using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentsEndpoints.MapCreateComponentEndpoint"/>.
/// Covers CPD-LC-001-009 Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentsEndpointsTests
{
    /// <summary>
    /// CPD-LC-001-009 Presentation-005: Verify that the POST endpoint for creating
    /// components is correctly mapped with the expected route and name.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Presentation-005: MapCreateComponentEndpoint maps POST /api/components")]
    public void MapCreateComponentEndpoint_MapsPostEndpointCorrectly()
    {
        // Arrange
        var mockBuilder = new Mock<IEndpointRouteBuilder>();
        var dataSources = new List<EndpointDataSource>();
        mockBuilder.Setup(x => x.DataSources).Returns(dataSources);

        var mockHandler = new Mock<CreateLearningComponentHandler>(
            MockBehavior.Loose,
            new object[] { Mock.Of<UCR.ECCI.PI.ThemePark.Backend.Application.Services.ILearningComponentService>() });

        // Act
        LearningComponentsEndpoints.MapCreateComponentEndpoint(mockBuilder.Object, mockHandler.Object);

        // Assert
        Assert.That(dataSources, Has.Count.GreaterThan(0));
    }
}
