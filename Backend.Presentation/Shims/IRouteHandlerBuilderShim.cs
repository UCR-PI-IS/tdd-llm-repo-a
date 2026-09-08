using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Shim interface for IRouteHandlerBuilder to support testing in .NET 8.
/// This interface is used by the test project to mock route handler builder operations.
/// </summary>
public interface IRouteHandlerBuilder
{
    /// <summary>
    /// Adds a name to the endpoint.
    /// </summary>
    /// <param name="name">The name to assign to the endpoint.</param>
    /// <returns>The route handler builder instance for chaining.</returns>
    IRouteHandlerBuilder WithName(string name);
}
