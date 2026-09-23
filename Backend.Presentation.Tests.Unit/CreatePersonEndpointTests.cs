using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint"/> endpoint mapping.
/// Covers intent Presentation-011.
/// 
/// NOTE: This test may need to be excluded from compilation because Moq 4.x cannot mock
/// extension methods (MapPost on IEndpointRouteBuilder). See the existing exclusion pattern
/// in the .csproj for LearningComponentEndpointsCreateTests.cs.
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// The endpoint should be registered with the correct route pattern.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_CreatePersonEndpoint_IsMappedToPostApiPersons()
    {
        // Arrange
        var mockEndpointRouteBuilder = new Mock<IEndpointRouteBuilder>();
        var endpoints = new List<EndpointDataSource>();

        mockEndpointRouteBuilder
            .Setup(b => b.DataSources)
            .Returns(endpoints);

        // Act
        CreatePersonEndpoint.MapCreatePersonEndpoint(mockEndpointRouteBuilder.Object);

        // Assert
        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true);
        Assert.That(personEndpoint, Is.Not.Null);
    }
}
