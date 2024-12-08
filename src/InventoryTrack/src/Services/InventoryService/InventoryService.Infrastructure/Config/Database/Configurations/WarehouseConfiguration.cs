using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryService.Infrastructure.Config.Database.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.Type)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.Location)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(w => w.CompanyId)
            .IsRequired();

        builder.Property(w => w.ResponsiblePersonId)
            .IsRequired();
    }
}