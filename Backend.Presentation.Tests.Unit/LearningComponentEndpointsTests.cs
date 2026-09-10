using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints.MapCreateComponentEndpoint"/>.
/// Covers intent Presentation-005 for story CPD-LC-001-009.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    private Mock<IEndpointRouteBuilder> _builder = null!;
    private Mock<ICreateLearningComponentHandler> _mockHandler = null!;

    [SetUp]
    public void SetUp()
    {
        _builder = new Mock<IEndpointRouteBuilder>();
        _mockHandler = new Mock<ICreateLearningComponentHandler>();
    }

    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components is correctly mapped.
    /// </summary>
    [Test]
    [Description("Presentation-005: Verify that the POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpoint()
    {
        // Arrange
        var dataSources = new List<EndpointDataSource>();
        _builder.Setup(x => x.DataSources).Returns(dataSources);

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(_builder.Object, _mockHandler.Object);

        // Assert
        Assert.That(dataSources.Count, Is.EqualTo(1));
        Assert.That(result, Is.SameAs(_builder.Object));
    }
}
