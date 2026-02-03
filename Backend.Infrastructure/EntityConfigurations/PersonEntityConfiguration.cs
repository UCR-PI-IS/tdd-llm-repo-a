using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for the Person entity.
/// </summary>
internal class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
{
    /// <summary>
    /// Configures the Person entity mapping.
    /// </summary>
    /// <param name="builder">The builder to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(p => p.Email)
            .IsUnique();

        builder.Property(p => p.IdentityNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(p => p.IdentityNumber)
            .IsUnique();

        builder.Property(p => p.BirthDate)
            .IsRequired();

        builder.Property(p => p.Phone)
            .HasMaxLength(20);
    }
}
