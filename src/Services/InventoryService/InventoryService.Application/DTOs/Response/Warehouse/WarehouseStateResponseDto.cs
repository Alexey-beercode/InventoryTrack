﻿using InventoryService.Application.DTOs.Base;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.WarehouseType;

namespace InventoryService.Application.DTOs.Response.Warehouse;

public class WarehouseStateResponseDto : BaseDto
{
    public string Name { get; set; }
    public WarehouseTypeResponseDto Type { get; set; }
    public int ItemsCount { get; set; }
    public string Location { get; set; }
    public long Quantity { get; set; }
    public List<InventoryItemResponseDto> InventoryItems { get; set; }
    public Guid ResponsiblePersonId { get; set; }
}