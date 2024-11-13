using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovementService.Domain.Entities;

namespace MovementService.Infrastructure.Config.Database.Configurations;

public class MovementRequestConfiguration : IEntityTypeConfiguration<MovementRequest>
{
    public void Configure(EntityTypeBuilder<MovementRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ItemId)
            .IsRequired();

        builder.Property(x => x.SourceWarehouseId)
            .IsRequired();

        builder.Property(x => x.DestinationWarehouseId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnType("varchar(255)")
            .IsRequired();

        builder.Property(x => x.RequestDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.ApprovedByUserId)
            .IsRequired(false);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);
    }
}