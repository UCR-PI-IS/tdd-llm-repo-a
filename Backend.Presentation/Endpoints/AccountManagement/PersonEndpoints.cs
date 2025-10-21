using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Handlers.AccountManagement;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Endpoints.AccountManagement;

/// <summary>
/// Defines the endpoints related to person management.
/// </summary>
public static class PersonEndpoints
{
    /// <summary>
    /// Maps the endpoints related to persons into the application.
    /// </summary>
    /// <param name="builder">The route builder to add endpoints to.</param>
    /// <returns>The route builder with the mapped endpoints.</returns>
    public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder builder)
    {

        builder.MapGet("/person/{identityNumber}", GetPersonByIdentityHandler.HandleAsync)
            .WithName("GetPersonByIdentity")
            .WithTags("Person Management")
            .WithOpenApi();

        builder.MapGet("/person/list", GetAllPeopleHandler.HandleAsync)
            .WithName("GetAllPeople")
            .WithTags("Person Management")
            .WithOpenApi();

        builder.MapPost("/person/create", PostCreatePersonHandler.HandleAsync)
            .WithName("PostCreatePerson")
            .WithTags("Person Management")
            .WithOpenApi();

        builder.MapPut("/person/modify/{identityNumber}", PutModifyPersonHandler.HandleAsync)
            .WithName("PutModifyPerson")
            .WithTags("Person Management")
            .WithOpenApi();

        builder.MapDelete("/person/delete/{identityNumber}", DeletePersonHandler.HandleAsync)
            .WithName("DeletePerson")
            .WithTags("Person Management")
            .WithOpenApi();

        

        return builder;
    }
}
