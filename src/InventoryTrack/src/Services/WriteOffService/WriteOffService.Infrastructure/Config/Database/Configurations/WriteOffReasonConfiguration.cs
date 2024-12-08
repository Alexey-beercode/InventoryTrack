using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Infrastructure.Config.Database.Configurations;

public class WriteOffReasonConfiguration : IEntityTypeConfiguration<WriteOffReason>
{
    public void Configure(EntityTypeBuilder<WriteOffReason> builder)
    {
        builder.ToTable("WriteOffReasons");

        builder.Property(x => x.Reason)
            .IsRequired()
            .HasMaxLength(255);
    }
}