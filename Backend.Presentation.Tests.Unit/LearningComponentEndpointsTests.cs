using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints.MapCreateComponentEndpoint"/>.
/// Covers intent Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components is correctly mapped.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify POST endpoint for creating components is mapped correctly")]
    public void MapCreateComponentEndpoint_MapsPostEndpoint()
    {
        // Arrange
        var app = WebApplication.Create();
        var mockHandler = new Mock<ICreateLearningComponentHandler>();

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(app, mockHandler.Object);

        // Assert
        Assert.That(result, Is.SameAs(app));
        var route = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .OfType<RouteEndpoint>()
            .FirstOrDefault(e => e.RoutePattern.RawText == "/api/components");
        Assert.That(route, Is.Not.Null, "Expected /api/components route to be registered");
    }
}
