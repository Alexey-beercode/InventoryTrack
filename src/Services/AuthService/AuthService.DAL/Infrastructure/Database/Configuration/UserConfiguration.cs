using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.Infrastructure.Database.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Login).IsRequired().HasMaxLength(100);
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(e => e.CompanyId);
    }
}