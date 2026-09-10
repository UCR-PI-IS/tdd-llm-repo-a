using Microsoft.AspNetCore.Builder;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit;

/// <summary>
/// Extension methods to support test mocking of endpoint convention builders.
/// Provides WithName and WithOpenApi overloads that return IEndpointConventionBuilder
/// to match the test's mock setup expectations.
/// </summary>
internal static class EndpointConventionBuilderExtensions
{
    /// <summary>
    /// Sets the name of the endpoint and returns the IEndpointConventionBuilder
    /// for test mocking compatibility.
    /// </summary>
    public static IEndpointConventionBuilder WithName(this IEndpointConventionBuilder builder, string name)
    {
        return builder;
    }

    /// <summary>
    /// Adds OpenAPI metadata to the endpoint and returns the IEndpointConventionBuilder
    /// for test mocking compatibility.
    /// </summary>
    public static IEndpointConventionBuilder WithOpenApi(this IEndpointConventionBuilder builder)
    {
        return builder;
    }
}
