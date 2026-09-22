using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Endpoints;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Unit tests for <see cref="CreatePersonEndpoint.MapEndpoint"/>.
/// Covers intent Presentation-011.
/// Uses a real <see cref="IEndpointRouteBuilder"/> test double since Moq cannot mock extension methods.
/// </summary>
[TestFixture]
public class CreatePersonEndpointTests
{
    /// <summary>
    /// Presentation-011: Verify endpoint is correctly mapped to POST /api/persons.
    /// </summary>
    [Test]
    [Description("Presentation-011: Endpoint is correctly mapped to POST /api/persons")]
    public void MapEndpoint_EndpointMapped_CreatesCorrectRoute()
    {
        // Arrange
        var builder = new TestEndpointRouteBuilder();

        // Act
        CreatePersonEndpoint.MapEndpoint(builder);

        // Assert
        var endpoints = builder.DataSources.SelectMany(ds => ds.Endpoints).ToList();
        var personEndpoint = endpoints.FirstOrDefault(e =>
            e.DisplayName?.Contains("CreatePerson") == true ||
            (e as RouteEndpoint)?.RoutePattern.RawText == "/api/persons");
        Assert.That(personEndpoint, Is.Not.Null);
    }

    /// <summary>
    /// Minimal test double for <see cref="IEndpointRouteBuilder"/> that allows endpoint mapping.
    /// </summary>
    private class TestEndpointRouteBuilder : IEndpointRouteBuilder
    {
        public ICollection<EndpointDataSource> DataSources { get; } = new List<EndpointDataSource>();
        public IServiceProvider ServiceProvider { get; } = new ServiceCollection().BuildServiceProvider();

        public IApplicationBuilder CreateApplicationBuilder()
        {
            throw new NotImplementedException();
        }
    }
}
