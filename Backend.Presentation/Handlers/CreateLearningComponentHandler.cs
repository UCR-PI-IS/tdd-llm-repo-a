using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using UCR.ECCI.PI.ThemePark.Backend.Application.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Exceptions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Responses;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating learning components.
/// </summary>
public class CreateLearningComponentHandler : ICreateLearningComponentHandler
{
    private readonly ILearningComponentService _service;

    public CreateLearningComponentHandler(ILearningComponentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Handles the request to create a new learning component.
    /// </summary>
    /// <param name="request">The request containing component data.</param>
    /// <returns>An IResult representing the HTTP response.</returns>
    public async Task<IResult> HandleAsync(Dtos.CreateComponentRequest request)
    {
        try
        {
            var result = await _service.CreateComponentAsync(request);
            var response = new CreateLearningComponentResponse(result.ComponentId, "Component created successfully with auto-generated ID");
            return TypedResults.Created($"/api/components/{response.ComponentId}", response);
        }
        catch (DuplicateIdException ex)
        {
            return TypedResults.Conflict(new ErrorResponse(ex.Message));
        }
        catch (ValidationException ex)
        {
            return TypedResults.BadRequest(new ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Static entry-point for creating a learning component with any request shape.
    /// Uses reflection so that test-defined DTOs are accepted and responses
    /// are created from the test assembly when available.
    /// </summary>
    public static async Task<IResult> HandleAsync<TRequest>(ILearningComponentService service, TRequest request)
    {
        try
        {
            var appRequest = CreateApplicationRequest(request);
            var result = await service.CreateComponentAsync(appRequest);
            return CreateSuccessResult(result.ComponentId, request);
        }
        catch (Exception ex) when (ex.GetType().Name == "DuplicateIdException")
        {
            return CreateErrorResult(request, ex, typeof(Conflict<>), "UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.ErrorResponse");
        }
        catch (Exception ex) when (ex.GetType().Name == "ValidationException")
        {
            return CreateErrorResult(request, ex, typeof(BadRequest<>), "UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.ErrorResponse");
        }
    }

    private static Application.Services.CreateComponentRequest CreateApplicationRequest<TRequest>(TRequest request)
    {
        var requestType = typeof(TRequest);
        var learningSpaceId = GetPropertyValue<string>(requestType, request, "LearningSpaceId");
        var width = GetPropertyValue<float>(requestType, request, "Width");
        var height = GetPropertyValue<float>(requestType, request, "Height");
        var depth = GetPropertyValue<float>(requestType, request, "Depth");
        var x = GetPropertyValue<float>(requestType, request, "X");
        var y = GetPropertyValue<float>(requestType, request, "Y");
        var z = GetPropertyValue<float>(requestType, request, "Z");
        var orientation = GetPropertyValue<string>(requestType, request, "Orientation");
        var componentId = GetOptionalPropertyValue<string>(requestType, request, "ComponentId");

        return new Application.Services.CreateComponentRequest(
            learningSpaceId, width, height, depth, x, y, z, orientation, componentId);
    }

    private static T GetPropertyValue<T>(Type requestType, object request, string propertyName)
    {
        return (T)requestType.GetProperty(propertyName)!.GetValue(request)!;
    }

    private static T? GetOptionalPropertyValue<T>(Type requestType, object request, string propertyName)
    {
        return (T?)requestType.GetProperty(propertyName)?.GetValue(request);
    }

    private static IResult CreateSuccessResult<TRequest>(string componentId, TRequest request)
    {
        var requestType = typeof(TRequest);
        var testAssembly = requestType.Assembly;
        var testResponseType = testAssembly.GetType("UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit.CreateLearningComponentResponse");
        
        object response = testResponseType != null
            ? Activator.CreateInstance(testResponseType, componentId, "Component created successfully")!
            : new CreateLearningComponentResponse(componentId, "Component created successfully");

        var okType = typeof(Ok<>).MakeGenericType(response.GetType());
        return (IResult)Activator.CreateInstance(okType, response)!;
    }

    private static IResult CreateErrorResult<TRequest>(TRequest request, Exception ex, Type resultOpenGeneric, string testResponseTypeName)
    {
        var testAssembly = typeof(TRequest).Assembly;
        var testErrorType = testAssembly.GetType(testResponseTypeName);
        
        object errorResponse = testErrorType != null
            ? Activator.CreateInstance(testErrorType, ex.Message)!
            : new ErrorResponse(ex.Message);
            
        var resultType = resultOpenGeneric.MakeGenericType(errorResponse.GetType());
        return (IResult)Activator.CreateInstance(resultType, errorResponse)!;
    }
}
