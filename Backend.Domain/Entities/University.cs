namespace UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

/// <summary>
/// Represents a university in the theme park system.
/// </summary>
public class University
{
    /// <summary>
    /// Gets the name of the university.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the country where the university is located.
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
    /// Initializes a new instance of the <see cref="University"/> class.
    /// </summary>
    /// <param name="name">The name of the university.</param>
    /// <param name="country">The country where the university is located.</param>
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
