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
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(BeAValidEmail).WithMessage("A valid email address is required.");

        RuleFor(x => x.IdentityNumber)
            .NotEmpty().WithMessage("Identity number is required.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today).WithMessage("Birth date must be in the past.");
    }

    private static bool BeAValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Must have local part, @, domain with at least one dot
        var atIndex = email.IndexOf('@');
        if (atIndex <= 0)
            return false;

        var domain = email[(atIndex + 1)..];
        if (string.IsNullOrEmpty(domain))
            return false;

        var dotIndex = domain.IndexOf('.');
        if (dotIndex <= 0 || dotIndex >= domain.Length - 1)
            return false;

        return true;
    }
}
