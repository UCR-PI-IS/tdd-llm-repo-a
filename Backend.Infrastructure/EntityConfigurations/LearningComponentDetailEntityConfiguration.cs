using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UCR.ECCI.PI.ThemePark.Backend.Domain.Entities;

namespace UCR.ECCI.PI.ThemePark.Backend.Infrastructure.EntityConfigurations;

/// <summary>
/// Entity configuration for the LearningComponentDetail entity.
/// </summary>
internal class LearningComponentDetailEntityConfiguration : IEntityTypeConfiguration<LearningComponentDetail>
{
    /// <summary>
    /// Configures the LearningComponentDetail entity.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<LearningComponentDetail> builder)
    {
        builder.ToTable("LearningComponentDetails");

        builder.HasKey(lcd => lcd.DetailId);

        builder.Property(lcd => lcd.DetailId)
            .IsRequired();

        builder.Property(lcd => lcd.ComponentId)
            .IsRequired();

        builder.Property(lcd => lcd.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(lcd => lcd.Metadata)
            .HasMaxLength(1000);

        builder.HasOne(lcd => lcd.Component)
            .WithMany()
            .HasForeignKey(lcd => lcd.ComponentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
