using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="LearningComponentEndpoints"/>.
/// Covers intent Presentation-005.
/// </summary>
[TestFixture]
public class LearningComponentEndpointsTests
{
    /// <summary>
    /// Presentation-005: Verify that the POST endpoint for creating components is correctly mapped.
    /// </summary>
    [Test]
    [Description("Presentation-005: POST endpoint for creating components is correctly mapped")]
    public void MapCreateComponentEndpoint_MapsPostEndpoint()
    {
        // Arrange
        var mockBuilder = new Mock<IEndpointRouteBuilder>();
        var mockHandler = new Mock<CreateLearningComponentHandler>(
            Mock.Of<ILearningComponentService>());

        var dataSources = new Microsoft.AspNetCore.Routing.EndpointDataSource[] { };
        var sourceList = new System.Collections.Generic.List<EndpointDataSource>();
        mockBuilder.Setup(b => b.DataSources).Returns(sourceList);

        // Act
        var result = LearningComponentEndpoints.MapCreateComponentEndpoint(
            mockBuilder.Object, mockHandler.Object);

        // Assert
        Assert.That(result, Is.SameAs(mockBuilder.Object));
    }
}
