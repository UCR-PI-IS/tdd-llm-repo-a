namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a university in the theme park system.
/// </summary>
public class University
{
    /// <summary>
    /// Unique identifier for the university.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Name of the university.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Country where the university is located.
    /// </summary>
    public string Country { get; private set; }

    /// <summary>
    /// Parameterless constructor for EF Core materialization.
    /// </summary>
    private University()
    {
        Name = string.Empty;
        Country = string.Empty;
    }

    /// <summary>
    /// Constructor for creating a new University with required fields.
    /// </summary>
    /// <param name="name">Name of the university.</param>
    /// <param name="country">Country where the university is located.</param>
    /// <exception cref="ArgumentException">Thrown when name or country is null or empty.</exception>
    public University(string name, string country)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required", nameof(country));

        Name = name;
        Country = country;
    }
}
