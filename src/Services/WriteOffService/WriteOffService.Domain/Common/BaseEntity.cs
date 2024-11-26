using InventoryService.Domain.Interfaces.Entities;

namespace WriteOffService.Domain.Common;

public class BaseEntity : IHasId,ISoftDeletable
{
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}