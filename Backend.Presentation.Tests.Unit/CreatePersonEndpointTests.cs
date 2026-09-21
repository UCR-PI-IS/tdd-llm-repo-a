using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint.MapEndpoint"/>.
/// Covers intent Presentation-011.
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_EndpointMapped_CreatesPostRoute()
    {
        // Arrange
        var mockEndpointConventionBuilder = new Mock<IEndpointConventionBuilder>();
        var mockRouteHandlerBuilder = new Mock<IRouteHandlerBuilder>();
        var mockEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();

        // Setup the route builder to return a mock RouteHandlerBuilder
        mockEndpointRouteBuilder
            .Setup(e => e.MapPost("/api/persons", It.IsAny<Delegate>()))
            .Returns(mockRouteHandlerBuilder.Object);

        // Act
        CreatePersonEndpoint.MapEndpoint(mockEndpointRouteBuilder.Object);

        // Assert
        mockEndpointRouteBuilder.Verify(e => e.MapPost("/api/persons", It.IsAny<Delegate>()), Times.Once);
    }
}
