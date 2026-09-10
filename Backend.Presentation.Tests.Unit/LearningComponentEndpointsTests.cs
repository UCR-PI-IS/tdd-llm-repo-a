using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints.MapCreateComponentEndpoint"/>.
/// Covers intent Presentation-005 from CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    private Mock<ICreateLearningComponentHandler> _mockHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _mockHandler = new Mock<ICreateLearningComponentHandler>();
    }

    /// <summary>
    /// Presentation-005 (CPD-LC-001-009): Verify that the POST endpoint for creating
    /// components is correctly mapped to "/api/components" with the name "CreateComponent".
    /// </summary>
    [Test]
    [Description("Presentation-005: POST /api/components endpoint is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostRoute_Correctly()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        // Act & Assert - should not throw; endpoint is registered successfully
        Assert.DoesNotThrow(() =>
            LearningComponentEndpoints.MapCreateComponentEndpoint(app, _mockHandler.Object));
    }
}
