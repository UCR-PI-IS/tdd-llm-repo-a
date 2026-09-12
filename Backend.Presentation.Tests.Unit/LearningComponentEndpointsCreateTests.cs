using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints.MapCreateComponentEndpoint"/>.
/// Covers intent Presentation-005 for CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsCreateTests
{
    private Mock<IEndpointRouteBuilder> _mockBuilder = null!;
    private Mock<IRouteHandlerBuilder> _mockRouteHandlerBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _mockBuilder = new Mock<IEndpointRouteBuilder>();
        _mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();

        _mockBuilder
            .Setup(x => x.MapPost(It.IsAny<string>(), It.IsAny<Delegate>()))
            .Returns(_mockRouteHandlerBuilder.Object);

        _mockRouteHandlerBuilder
            .Setup(x => x.WithName(It.IsAny<string>()))
            .Returns(_mockRouteHandlerBuilder.Object);
        _mockRouteHandlerBuilder
            .Setup(x => x.WithOpenApi())
            .Returns(_mockRouteHandlerBuilder.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockBuilder.VerifyAll();
        _mockRouteHandlerBuilder.VerifyAll();
    }

    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components is correctly mapped.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify that the POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpoint()
    {
        // Arrange
        var mockHandler = new Mock<ILearningComponentService>();

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(_mockBuilder.Object, mockHandler.Object);

        // Assert
        Assert.Multiple(() =>
        {
            _mockBuilder.Verify(x => x.MapPost("/api/components", It.IsAny<Delegate>()), Times.Once);
            _mockRouteHandlerBuilder.Verify(x => x.WithName("CreateComponent"), Times.Once);
        });
    }
}
