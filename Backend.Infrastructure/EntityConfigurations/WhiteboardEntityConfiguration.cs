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

        // Primary key configuration
        builder.HasKey(wb => wb.ComponentId);

        // Property attribute configurations
        builder.Property(wb => wb.ComponentId)
            .HasMaxLength(50);

        builder.Property(wb => wb.LearningSpaceId)
            .HasMaxLength(50);

        builder.Property(wb => wb.Orientation)
            .HasMaxLength(20);

        builder.Property(wb => wb.MarkerColor)
            .HasMaxLength(50);

        builder.Property(wb => wb.Width);
        builder.Property(wb => wb.Height);
        builder.Property(wb => wb.Depth);
        builder.Property(wb => wb.X);
        builder.Property(wb => wb.Y);
        builder.Property(wb => wb.Z);
    }
}
