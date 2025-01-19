using IMS.Domain.Aggregates;
using IMS.Domain.ValueObjects;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.IsPerishable)
            .IsRequired();

        builder.Property(x => x.QualityStatus)
            .IsRequired();

        builder.OwnsOne(x => x.SKU, sku =>
        {
            sku.Property(s => s.Value)
                .HasColumnName("SKU")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.OwnsOne(x => x.StockLevel, sl =>
        {
            sl.Property(s => s.Current)
                .HasColumnName("CurrentStock")
                .IsRequired();

            sl.Property(s => s.Minimum)
                .HasColumnName("MinimumStock")
                .IsRequired();

            sl.Property(s => s.Maximum)
                .HasColumnName("MaximumStock")
                .IsRequired();
            sl.Property(s => s.Critical)
                .HasColumnName("Critical")
                .IsRequired();
        });

        // Configure StorageLocations as a collection of strings
        builder.Property(x => x.StorageLocations)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("StorageLocations");


        builder.Ignore(x => x.DomainEvents);
    }
}
