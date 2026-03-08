using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for the Whiteboard entity.
/// </summary>
internal class WhiteboardEntityConfiguration : IEntityTypeConfiguration<Whiteboard>
{
    /// <summary>
    /// Configures the Whiteboard entity mapping.
    /// </summary>
    /// <param name="builder">The builder to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Whiteboard> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Width)
            .IsRequired();

        builder.Property(w => w.Height)
            .IsRequired();
    }
}
