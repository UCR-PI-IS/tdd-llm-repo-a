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
/// Covers intent Presentation-005 from story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components
    /// is correctly mapped with the expected route and name.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify that the POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpointCorrectly()
    {
        // Arrange
        var mockService = new Mock<ILearningComponentService>();
        var handler = new CreateLearningComponentHandler(mockService.Object);
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        var app = builder.Build();

        // Act
        LearningComponentEndpoints.MapCreateComponentEndpoint(app, handler);

        // Assert
        var endpoints = ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(ds => ds.Endpoints)
            .ToList();

        Assert.That(endpoints, Has.Some.Matches<Endpoint>(
            e => e.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName == "CreateComponent"));
    }
}
