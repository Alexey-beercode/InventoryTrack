using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Config.Database.Configurations
{
    public class InventoriesItemsWarehousesConfiguration : IEntityTypeConfiguration<InventoriesItemsWarehouses>
    {
        public void Configure(EntityTypeBuilder<InventoriesItemsWarehouses> builder)
        {
            // Table name
            builder.ToTable("InventoriesItemsWarehouses");

            // Primary Key
            builder.HasKey(iw => iw.Id);

            // Foreign Key: InventoryItem
            builder.HasOne(iw => iw.InventoryItem)
                .WithMany()
                .HasForeignKey(iw => iw.ItemId)
                .OnDelete(DeleteBehavior.Cascade) // Adjust based on your requirements
                .IsRequired();

            // Foreign Key: Warehouse
            builder.HasOne(iw => iw.Warehouse)
                .WithMany()
                .HasForeignKey(iw => iw.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict) // Adjust based on your requirements
                .IsRequired();

            // Additional constraints
            builder.Property(iw => iw.Quantity)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}