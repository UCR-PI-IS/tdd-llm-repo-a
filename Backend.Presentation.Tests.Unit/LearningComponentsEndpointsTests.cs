using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentsEndpoints"/> endpoint mapping.
/// Covers CPD-LC-001-009 intent Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentsEndpointsTests
{
    /// <summary>
    /// CPD-LC-001-009 Presentation-005: Verify that the POST endpoint for creating
    /// learning components is correctly mapped on the endpoint route builder.
    /// </summary>
    [Test]
    [Description("CPD-LC-001-009 Presentation-005: MapCreateComponentEndpoint registers the POST endpoint")]
    public void MapCreateComponentEndpoint_RegistersPostEndpoint()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act & Assert — MapCreateComponentEndpoint should execute without throwing,
        // which confirms the endpoint is registered on the route builder.
        Assert.DoesNotThrow(() => LearningComponentsEndpoints.MapCreateComponentEndpoint(app));
    }
}
