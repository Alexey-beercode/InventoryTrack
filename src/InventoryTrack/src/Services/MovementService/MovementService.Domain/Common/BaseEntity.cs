using MovementService.Domain.Interfaces.Entities;

namespace MovementService.Domain.Common;

public class BaseEntity : IHasId,ISoftDeletable
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}