using FluentValidation;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Validator for <see cref="CreatePersonDto"/>.
/// </summary>
public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePersonDtoValidator"/> class.
    /// </summary>
    public CreatePersonDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Matches(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").WithMessage("A valid email address is required.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("Identity number is required.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");
    }
}
