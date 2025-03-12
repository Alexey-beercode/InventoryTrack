using WriteOffService.Domain.Enums;

namespace WriteOffService.Domain.Models;

public class FilterWriteOffrequestModel
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Guid ReasonId { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.None;
    public DateTime RequestDate { get; set; }
    public Guid? ApprovedByUserId { get; set; } // ✅ Теперь null значит "получить все"
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsPaginated { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}