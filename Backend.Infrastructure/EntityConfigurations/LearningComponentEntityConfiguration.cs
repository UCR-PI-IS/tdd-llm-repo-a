using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="LearningComponent"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class LearningComponentEntityConfiguration : IEntityTypeConfiguration<LearningComponent>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="LearningComponent"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LearningComponent> builder)
    {
        builder.ToTable("LearningComponent");

        // Primary key configuration
        builder.HasKey(lc => lc.ComponentId);

        // Property attribute configurations
        builder.Property(lc => lc.ComponentId)
            .HasMaxLength(50);

        builder.Property(lc => lc.LearningSpaceId)
            .HasMaxLength(50);

        builder.Property(lc => lc.Orientation)
            .HasMaxLength(20);

        // Use REAL for float properties (SQL Server REAL is 4-byte float)
        builder.Property(lc => lc.Width)
            .HasColumnType("real");
        builder.Property(lc => lc.Height)
            .HasColumnType("real");
        builder.Property(lc => lc.Depth)
            .HasColumnType("real");
        builder.Property(lc => lc.X)
            .HasColumnType("real");
        builder.Property(lc => lc.Y)
            .HasColumnType("real");
        builder.Property(lc => lc.Z)
            .HasColumnType("real");
    }
}
