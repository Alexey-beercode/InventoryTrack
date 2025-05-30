﻿using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

public class Supplier : BaseEntity
{
    public Guid CompanyId { get; set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string PostalAddress { get; set; }
    public string AccountNumber { get; set; }
}