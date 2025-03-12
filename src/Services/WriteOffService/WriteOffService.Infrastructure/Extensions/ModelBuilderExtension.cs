using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Infrastructure.Extensions;

public static class ModelBuilderExtension
{
    public static void SeedWriteOffsReasonsData(this ModelBuilder modelBuilder)
    {
        var writeOffReason1 = new WriteOffReason()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Reason = "По причине продажи"
        };
        var writeOffReason2 = new WriteOffReason()
                {
                    Id = Guid.NewGuid(),
                    IsDeleted = false,
                    Reason = "Истёк срок годности"
                };
        var writeOffReason3 = new WriteOffReason()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Reason = "Поломка"
        };

        var writeOffReason4 = new WriteOffReason()
        {
            Id = Guid.NewGuid(),
            IsDeleted = false,
            Reason = "Другое"
        };
        
   
        modelBuilder.Entity<WriteOffReason>().HasData(writeOffReason1);
        modelBuilder.Entity<WriteOffReason>().HasData(writeOffReason2);
        modelBuilder.Entity<WriteOffReason>().HasData(writeOffReason3);
        modelBuilder.Entity<WriteOffReason>().HasData(writeOffReason4);
    }
}