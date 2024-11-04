using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.DAL.Infrastructure.Database.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(e => e.UserId);
        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(e => e.RoleId);
    }
}