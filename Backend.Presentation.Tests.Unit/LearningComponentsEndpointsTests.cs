using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentsEndpoints"/> endpoint mapping.
/// Covers CPD-LC-001-009 intent Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentsEndpointsTests
{
    /// <summary>
    /// CPD-LC-001-009 Presentation-005: Verify that the POST endpoint for creating components
    /// is correctly mapped with the expected route and name.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Presentation-005: POST /api/components endpoint is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostRouteCorrectly()
    {
        // Arrange
        var mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();
        mockRouteHandlerBuilder
            .Setup(x => x.WithName(It.IsAny<string>()))
            .Returns(mockRouteHandlerBuilder.Object);

        var mockBuilder = new Mock<IEndpointRouteBuilder>();
        mockBuilder
            .Setup(x => x.MapPost(It.IsAny<string>(), It.IsAny<Delegate>()))
            .Returns(mockRouteHandlerBuilder.Object);

        var mockHandler = new Mock<CreateLearningComponentHandler>(MockBehavior.Loose,
            new object[] { Mock.Of<UCR.ECCI.PI.ThemePark.Backend.Application.Services.ILearningComponentService>() });

        // Act
        LearningComponentsEndpoints.MapCreateComponentEndpoint(mockBuilder.Object, mockHandler.Object);

        // Assert
        Assert.Multiple(() =>
        {
            mockBuilder.Verify(x => x.MapPost("/api/components", It.IsAny<Delegate>()), Times.Once);
            mockRouteHandlerBuilder.Verify(x => x.WithName("CreateComponent"), Times.Once);
        });
    }
}
