using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Infrastructure.Config.Database.Configurations;

public class WriteOffActConfiguration : IEntityTypeConfiguration<WriteOffAct>
{
    public void Configure(EntityTypeBuilder<WriteOffAct> builder)
    {
        builder.ToTable("WriteOffActs");

        builder.Property(x => x.WriteOffRequestId)
            .IsRequired();

        builder.Property(x => x.DocumentId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.WriteOffRequest)
            .WithMany()
            .HasForeignKey(x => x.WriteOffRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Document)
            .WithMany()
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}