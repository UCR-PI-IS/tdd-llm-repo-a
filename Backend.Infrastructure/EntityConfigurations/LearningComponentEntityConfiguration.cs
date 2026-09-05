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

        builder.Property(lc => lc.Width)
            .HasColumnType("REAL");
        builder.Property(lc => lc.Height)
            .HasColumnType("REAL");
        builder.Property(lc => lc.Depth)
            .HasColumnType("REAL");
        builder.Property(lc => lc.X)
            .HasColumnType("REAL");
        builder.Property(lc => lc.Y)
            .HasColumnType("REAL");
        builder.Property(lc => lc.Z)
            .HasColumnType("REAL");
    }
}
