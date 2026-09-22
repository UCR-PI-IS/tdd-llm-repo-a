using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Validator for <see cref="CreatePersonDto"/>.
/// </summary>
public class CreatePersonDtoValidator
{
    /// <summary>
    /// Validates the provided <see cref="CreatePersonDto"/>.
    /// </summary>
    public ValidationResult Validate(CreatePersonDto dto)
    {
        var errors = new List<ValidationFailure>();

        if (string.IsNullOrWhiteSpace(dto.FirstName))
            errors.Add(new ValidationFailure("FirstName", "First name is required."));

        if (string.IsNullOrWhiteSpace(dto.LastName))
            errors.Add(new ValidationFailure("LastName", "Last name is required."));

        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add(new ValidationFailure("Email", "Email is required."));
        else if (!IsValidEmail(dto.Email))
            errors.Add(new ValidationFailure("Email", "Email format is invalid."));

        if (string.IsNullOrWhiteSpace(dto.IdentityNumber))
            errors.Add(new ValidationFailure("IdentityNumber", "Identity number is required."));

        if (dto.BirthDate >= DateTime.Today)
            errors.Add(new ValidationFailure("BirthDate", "Birth date must be in the past."));

        return new ValidationResult(errors);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
