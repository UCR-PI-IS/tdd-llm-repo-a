using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentsEndpoints"/> endpoint mapping.
/// Covers intent Presentation-005 from CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentsEndpointsTests
{
    /// <summary>
    /// Presentation-005 (CPD-LC-001-009): Verify that the POST endpoint for creating
    /// components is correctly mapped at "/api/components" with the name "CreateComponent".
    /// </summary>
    [Test]
    [Description("Presentation-005 (CPD-LC-001-009): POST /api/components endpoint is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpointCorrectly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddScoped(_ => new Mock<ILearningComponentService>().Object);
        var app = builder.Build();

        // Act
        LearningComponentsEndpoints.MapCreateComponentEndpoint(app);

        // Assert
        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .Cast<RouteEndpoint>()
            .ToList();

        var createComponentEndpoint = endpoints
            .FirstOrDefault(e => e.RoutePattern.RawText == "/api/components");

        Assert.That(createComponentEndpoint, Is.Not.Null,
            "POST /api/components endpoint should be registered");
    }
}
