using Microsoft.AspNetCore.Builder;
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
    [Description("Presentation-011: Endpoint correctly mapped to POST /api/persons")]
    public void MapEndpoint_EndpointMapped_CreatesPostEndpoint()
    {
        // Arrange
        var mockBuilder = new Mock<IEndpointRouteBuilder>();
        var mockConventionBuilder = new Mock<IEndpointConventionBuilder>();
        
        mockBuilder
            .Setup(b => b.MapPost("/api/persons", It.IsAny<Delegate>()))
            .Returns(mockConventionBuilder.Object);

        // Act
        CreatePersonEndpoint.MapEndpoint(mockBuilder.Object);

        // Assert
        mockBuilder.Verify(b => b.MapPost("/api/persons", It.IsAny<Delegate>()), Times.Once);
    }
}
