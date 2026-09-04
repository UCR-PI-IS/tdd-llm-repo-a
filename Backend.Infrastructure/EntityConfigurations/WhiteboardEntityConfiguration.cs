using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="Whiteboard"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class WhiteboardEntityConfiguration : IEntityTypeConfiguration<Whiteboard>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="Whiteboard"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Whiteboard> builder)
    {
        builder.ToTable("Whiteboard");

        // Property attribute configurations
        builder.Property(w => w.ComponentId)
            .HasMaxLength(50);

        builder.Property(w => w.LearningSpaceId)
            .HasMaxLength(50);

        builder.Property(w => w.Orientation)
            .HasMaxLength(20);

        builder.Property(w => w.MarkerColor)
            .HasMaxLength(50);

        builder.Property(w => w.Width);
        builder.Property(w => w.Height);
        builder.Property(w => w.Depth);
        builder.Property(w => w.X);
        builder.Property(w => w.Y);
        builder.Property(w => w.Z);
    }
}
