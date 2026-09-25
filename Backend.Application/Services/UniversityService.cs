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
    /// <returns>A service result containing the created university or an error message.</returns>
    public async Task<ServiceResult<University>> AddUniversityAsync(string name, string country)
    {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
            return Failure("Name is required");

        // Validate country
        if (string.IsNullOrWhiteSpace(country))
            return Failure("Country is required");

        // Check for duplicate name
        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
            return Failure($"University with name '{name}' already exists");

        // Create and persist the university
        var university = new University(name, country);
        await _repository.AddAsync(university);

        return new ServiceResult<University> { IsSuccess = true, Data = university };
    }

    private static ServiceResult<University> Failure(string message) =>
        new ServiceResult<University> { IsSuccess = false, ErrorMessage = message };
}
