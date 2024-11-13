namespace MovementService.Domain.Interfaces.Entities;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
