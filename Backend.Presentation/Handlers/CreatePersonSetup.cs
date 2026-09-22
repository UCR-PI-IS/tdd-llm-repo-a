using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Handlers;

internal readonly record struct CreatePersonSetup(bool IsValidationError, Person? Person);
