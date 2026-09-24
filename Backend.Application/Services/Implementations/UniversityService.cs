using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Repositories;

namespace UCR.ECCI.PI.ThemePark.Backend.Application.Services.Implementations;

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
    /// <returns>A result indicating success or failure.</returns>
    public async Task<Result> AddUniversityAsync(string name, string country)
    {
        University university;
        try
        {
            university = new University(name, country);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }

        var exists = await _repository.ExistsByNameAsync(name);
        if (exists)
        {
            return Result.Failure($"University with name '{name}' already exists");
        }

        await _repository.AddAsync(university);
        return Result.Success();
    }
}
