using Microsoft.EntityFrameworkCore;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;
using UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.Data;

/// <summary>
/// Represents the database context for the Theme Park system.
/// Used for testing purposes and provides access to learning components.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with specified options.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of learning components in the database.
    /// </summary>
    public virtual DbSet<LearningComponent> LearningComponents { get; set; } = null!;

    /// <summary>
    /// Configures the model relationships and entity mappings when the model for a context is being created.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply the LearningComponent entity configuration
        modelBuilder.ApplyConfiguration(new LearningComponentEntityConfiguration());
    }
}
