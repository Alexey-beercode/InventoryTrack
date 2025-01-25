using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Config.Database.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.PhoneNumber)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.PostalAddress)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.AccountNumber)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(s => s.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.Property(w => w.CompanyId)
            .IsRequired();
    }
}