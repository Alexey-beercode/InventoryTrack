using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Infrastructure.Config.Database.Configurations;

public class WriteOffRequestConfiguration : IEntityTypeConfiguration<WriteOffRequest>
{
    public void Configure(EntityTypeBuilder<WriteOffRequest> builder)
    {
        builder.ToTable("WriteOffRequests");

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.WarehouseId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.ReasonId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.RequestDate)
            .IsRequired();

        // Relationship
        builder.HasOne(x => x.Reason)
            .WithMany()
            .HasForeignKey(x => x.ReasonId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ItemId);
        builder.HasIndex(x => x.WarehouseId);
        builder.HasIndex(x => x.Status);
    }
}