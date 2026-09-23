using FluentValidation;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Validator for <see cref="CreatePersonDto"/>.
/// Ensures all required fields are present and valid.
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
            .WithMessage("FirstName is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email => !string.IsNullOrEmpty(email) && email.Contains("@") && email.Split('@')[0].Length > 0 && email.Split('@')[1].Contains("."))
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty()
            .WithMessage("IdentityNumber is required.");

        RuleFor(x => x.BirthDate)
            .Must(birthDate => birthDate < DateTime.Today)
            .WithMessage("BirthDate must be a past date.");
    }
}
