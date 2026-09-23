using FluentValidation;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

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
            .WithMessage("FirstName is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email must be a valid email address.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty()
            .WithMessage("IdentityNumber is required.");

        RuleFor(x => x.BirthDate)
            .Must(birthDate => birthDate < DateTime.Today)
            .WithMessage("BirthDate must be a past date.");
    }
}
