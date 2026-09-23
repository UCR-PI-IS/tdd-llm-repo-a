using FluentValidation;
using Microsoft.AspNetCore.Http;
using UCR.ECCI.PI.ThemePark.Backend.Application.Services;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Mappers;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

/// <summary>
/// Handler for creating a new person.
/// </summary>
public class CreatePersonHandler
{
    private readonly IPersonService _service;
    private readonly IValidator<CreatePersonDto> _validator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonHandler"/> class.
    /// </summary>
    /// <param name="service">The person service.</param>
    /// <param name="validator">The DTO validator.</param>
    public CreatePersonHandler(IPersonService service, IValidator<CreatePersonDto> validator)
    {
        _service = service;
        _validator = validator;
    }

    /// <summary>
    /// Handles the asynchronous request to create a new person.
    /// </summary>
    /// <param name="dto">The data transfer object containing the creation parameters.</param>
    /// <returns>A handler result wrapping the HTTP result.</returns>
    public async Task<CreatePersonHandlerResult> HandleAsync(CreatePersonDto dto)
    {
        var validationResult = _validator.Validate(dto);
        if (!validationResult.IsValid)
        {
            return CreatePersonResponseFactory.CreateValidationErrorResponse(validationResult);
        }

        var person = PersonMapper.ToDomain(dto);
        var result = await _service.CreatePersonAsync(person);

        if (result.IsSuccess)
        {
            return CreatePersonResponseFactory.CreateSuccessResponse();
        }

        return CreatePersonResponseFactory.CreateConflictResponse(result.ErrorMessage!);
    }
}

/// <summary>
/// Wrapper for the handler result containing the HTTP result.
/// </summary>
public class CreatePersonHandlerResult
{
    /// <summary>
    /// Gets the HTTP result.
    /// </summary>
    public IResult Result { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonHandlerResult"/> class.
    /// </summary>
    /// <param name="result">The HTTP result.</param>
    public CreatePersonHandlerResult(IResult result)
    {
        Result = result;
    }
}
