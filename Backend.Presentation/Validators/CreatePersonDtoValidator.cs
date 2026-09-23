using System.Text.RegularExpressions;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Validators;

/// <summary>
/// Validator for <see cref="CreatePersonDto"/>.
/// </summary>
public class CreatePersonDtoValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    /// <summary>
    /// Validates the given <see cref="CreatePersonDto"/>.
    /// </summary>
    /// <param name="dto">The DTO to validate.</param>
    /// <returns>A validation result indicating success or failure with errors.</returns>
    public virtual DtoValidationResult Validate(CreatePersonDto dto)
    {
        var errors = new List<ValidationFailure>();

        ValidateRequired(dto.FirstName, "FirstName", errors);
        ValidateRequired(dto.LastName, "LastName", errors);
        ValidateEmail(dto.Email, errors);
        ValidateRequired(dto.IdentityNumber, "IdentityNumber", errors);
        ValidateBirthDate(dto.BirthDate, errors);

        return new DtoValidationResult(errors.Count == 0, errors);
    }

    private static void ValidateRequired(string value, string propertyName, List<ValidationFailure> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
            errors.Add(new ValidationFailure(propertyName, $"{propertyName} is required."));
    }

    private static void ValidateEmail(string email, List<ValidationFailure> errors)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(new ValidationFailure("Email", "Email is required."));
            return;
        }

        if (!EmailRegex.IsMatch(email))
            errors.Add(new ValidationFailure("Email", "Email format is invalid."));
    }

    private static void ValidateBirthDate(DateTime birthDate, List<ValidationFailure> errors)
    {
        if (birthDate > DateTime.Today)
            errors.Add(new ValidationFailure("BirthDate", "BirthDate must not be in the future."));
    }
}
