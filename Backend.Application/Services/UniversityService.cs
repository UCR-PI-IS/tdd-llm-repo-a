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
    /// <param name="country">The country of the university.</param>
    /// <returns>A service result indicating success or failure.</returns>
    public async Task<ServiceResult<object>> AddUniversityAsync(string name, string country)
    {
        // Validate name
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ServiceResult<object>
            {
                IsSuccess = false,
                ErrorMessage = "Name is required"
            };
        }

        // Validate country
        if (string.IsNullOrWhiteSpace(country))
        {
            return new ServiceResult<object>
            {
                IsSuccess = false,
                ErrorMessage = "Country is required"
            };
        }

        // Check for duplicate
        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
        {
            return new ServiceResult<object>
            {
                IsSuccess = false,
                ErrorMessage = $"University with name '{name}' already exists"
            };
        }

        // Create and add the university
        var university = new University(name, country);
        await _repository.AddAsync(university);

        return new ServiceResult<object>
        {
            IsSuccess = true
        };
    }
}
