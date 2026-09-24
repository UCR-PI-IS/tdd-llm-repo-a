using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services;

/// <summary>
/// Service for managing university operations.
/// </summary>
public class UniversityService : IUniversityService
{
    private readonly IUniversityRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversityService"/> class.
    /// </summary>
    /// <param name="repository">The university repository.</param>
    public UniversityService(IUniversityRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Adds a new university to the system.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country where the university is located.</param>
    /// <returns>A result indicating success or failure of the operation.</returns>
    public async Task<Result> AddUniversityAsync(string name, string country)
    {
        var validationError = ValidateInputs(name, country);
        if (validationError != null)
            return Result.Failure(validationError);

        // Check for duplicates
        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
            return Result.Failure($"University with name '{name}' already exists");

        // Create and add the university
        var university = new University(name, country);
        await _repository.AddAsync(university);

        return Result.Success();
    }

    /// <summary>
    /// Validates the input parameters for adding a university.
    /// </summary>
    /// <param name="name">The name to validate.</param>
    /// <param name="country">The country to validate.</param>
    /// <returns>An error message if validation fails, null otherwise.</returns>
    private static string? ValidateInputs(string name, string country)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Name is required";

        if (string.IsNullOrWhiteSpace(country))
            return "Country is required";

        return null;
    }
}
