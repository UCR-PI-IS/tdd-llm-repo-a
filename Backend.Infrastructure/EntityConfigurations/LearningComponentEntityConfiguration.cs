using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity Framework configuration for the LearningComponent entity.
/// </summary>
internal class LearningComponentEntityConfiguration : IEntityTypeConfiguration<LearningComponent>
{
    /// <summary>
    /// Configures the LearningComponent entity mapping.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LearningComponent> builder)
    {
        builder.ToTable("LearningComponent");

        builder.HasKey(lc => lc.ComponentId);

        builder.Property(lc => lc.ComponentId)
            .HasColumnName("ComponentId")
            .IsRequired();

        builder.Property(lc => lc.LearningSpaceId)
            .HasColumnName("LearningSpaceId")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(lc => lc.Width)
            .HasColumnName("Width")
            .IsRequired();

        builder.Property(lc => lc.Height)
            .HasColumnName("Height")
            .IsRequired();

        builder.Property(lc => lc.Depth)
            .HasColumnName("Depth")
            .IsRequired();

        builder.Property(lc => lc.X)
            .HasColumnName("X")
            .IsRequired();

        builder.Property(lc => lc.Y)
            .HasColumnName("Y")
            .IsRequired();

        builder.Property(lc => lc.Z)
            .HasColumnName("Z")
            .IsRequired();

        builder.Property(lc => lc.Orientation)
            .HasColumnName("Orientation")
            .IsRequired()
            .HasMaxLength(50);
    }
}
