using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Configuration class for the <see cref="LearningSpace"/> entity.
/// Maps the entity properties to the corresponding table and columns in the database.
/// </summary>
internal class LearningSpaceEntityConfiguration : IEntityTypeConfiguration<LearningSpace>
{
    /// <summary>
    /// Configures the entity framework mapping for the <see cref="LearningSpace"/> entity.
    /// </summary>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<LearningSpace> builder)
    {
        builder.ToTable("LearningSpace");

        // Primary key configuration - ID is database-generated
        builder.HasKey(ls => ls.LearningSpaceId);
        
        // Configure ID to be database-generated identity
        builder.Property(ls => ls.LearningSpaceId)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        // Property attribute configurations
        builder.Property(ls => ls.Type)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ls => ls.Height)
            .IsRequired();

        builder.Property(ls => ls.Width)
            .IsRequired();

        builder.Property(ls => ls.Length)
            .IsRequired();
    }
}
