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
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public University(string name, string country)
    {
        ValidateRequiredString(name, nameof(Name));
        ValidateRequiredString(country, nameof(Country));

        Name = name;
        Country = country;
    }

    private static void ValidateRequiredString(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} is required", paramName);
    }
}
