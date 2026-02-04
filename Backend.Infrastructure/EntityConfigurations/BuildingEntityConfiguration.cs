using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for the Building entity.
/// </summary>
internal class BuildingEntityConfiguration : IEntityTypeConfiguration<Building>
{
    /// <summary>
    /// Configures the Building entity mapping.
    /// </summary>
    /// <param name="builder">The builder to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Building> builder)
    {
        builder.HasKey(b => b.InternalId);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(b => b.Name)
            .IsUnique();

        builder.Property(b => b.Color)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Height)
            .IsRequired();

        builder.Property(b => b.Length)
            .IsRequired();

        builder.Property(b => b.Width)
            .IsRequired();

        builder.Property(b => b.X)
            .IsRequired();

        builder.Property(b => b.Y)
            .IsRequired();

        builder.Property(b => b.Z)
            .IsRequired();
    }
}
