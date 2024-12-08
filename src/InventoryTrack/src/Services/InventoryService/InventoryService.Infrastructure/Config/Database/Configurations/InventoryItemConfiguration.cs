using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Config.Database.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(i => i.UniqueCode)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.EstimatedValue)
            .HasColumnType("decimal(8, 2)")
            .IsRequired();

        builder.Property(i => i.ExpirationDate)
            .IsRequired();

        builder.Property(i => i.DeliveryDate)
            .IsRequired();

        builder.HasOne(i => i.Supplier)
            .WithMany()
            .HasForeignKey(i => i.SupplierId);

        builder.HasOne(i => i.Warehouse)
            .WithMany()
            .HasForeignKey(i => i.WarehouseId);

        builder.HasOne(i => i.Document)
            .WithMany()
            .HasForeignKey(i => i.DocumentId);
    }
}