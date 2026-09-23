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
            .WithMessage("FirstName is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .Must(email => !string.IsNullOrEmpty(email) && email.Contains("@") && email.Contains(".") && email.IndexOf("@") > 0 && email.LastIndexOf(".") > email.IndexOf("@"))
            .WithMessage("Email format is invalid.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty()
            .WithMessage("IdentityNumber is required.");

        RuleFor(x => x.BirthDate)
            .Must(birthDate => birthDate.Date < DateTime.Now.Date)
            .WithMessage("BirthDate must be a past date.");
    }
}
