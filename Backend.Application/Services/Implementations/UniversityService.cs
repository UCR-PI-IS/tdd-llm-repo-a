using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

/// <summary>
/// Service implementation for university-related operations.
/// </summary>
public class UniversityService : IUniversityService
{
    private readonly IUniversityRepository _universityRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversityService"/> class.
    /// </summary>
    /// <param name="universityRepository">The university repository.</param>
    public UniversityService(IUniversityRepository universityRepository)
    {
        _universityRepository = universityRepository;
    }

    /// <summary>
    /// Adds a new university with the specified name and country.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country where the university is located.</param>
    /// <returns>A <see cref="ServiceResult{T}"/> indicating success or failure.</returns>
    public async Task<ServiceResult<bool>> AddUniversityAsync(string name, string country)
    {
        if (string.IsNullOrEmpty(name))
            return Failure("Name is required");

        if (string.IsNullOrEmpty(country))
            return Failure("Country is required");

        var exists = await _universityRepository.ExistsByNameAsync(name);
        if (exists)
            return Failure($"University with name '{name}' already exists");

        var university = new University(name, country);
        await _universityRepository.AddAsync(university);

        return new ServiceResult<bool> { IsSuccess = true };
    }

    private static ServiceResult<bool> Failure(string errorMessage)
        => new() { IsSuccess = false, ErrorMessage = errorMessage };
}
