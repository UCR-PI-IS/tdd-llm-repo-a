using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;

/// <summary>
/// Represents the database context for the Theme Park system.
/// It handles the database operations and mappings for entity configurations.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// Protected parameterless constructor for mocking purposes.
    /// </summary>
    protected ApplicationDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class with specified options.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of learning components in the database.
    /// </summary>
    public virtual DbSet<LearningComponent> LearningComponents { get; private set; } = null!;
}
