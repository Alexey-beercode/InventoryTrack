using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.Infrastructure.Database.Configuration;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Unp).IsRequired();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.LegalAddress).HasMaxLength(500);
        builder.Property(e => e.PostalAddress).HasMaxLength(500);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.ResponsibleUserId);
    }
}