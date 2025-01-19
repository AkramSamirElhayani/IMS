using IMS.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Reference, reference =>
        {
            reference.Property(r => r.Value)
                .HasColumnName("Reference")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.SourceLocation)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.DestinationLocation)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.BatchInfo, bi =>
        {
            bi.Property(b => b.BatchNumber)
                .HasColumnName("BatchNumber")
                .HasMaxLength(50);
                
            bi.Property(b => b.ManufacturingDate)
                .HasColumnName("ManufactureDate");
                
            bi.Property(b => b.ExpiryDate)
                .HasColumnName("ExpiryDate");
        });

        builder.Property(x => x.TransactionDate)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
