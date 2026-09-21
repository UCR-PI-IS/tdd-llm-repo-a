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
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty()
            .WithMessage("Identity number is required.");

        RuleFor(x => x.BirthDate)
            .Must(BeInThePast)
            .WithMessage("Birth date must be in the past.");
    }

    private static bool BeInThePast(DateTime birthDate)
    {
        return birthDate < DateTime.Now.Date;
    }
}
