export class UpdateInventoryItemDto {
  warehouseId!: string;
  quantity!: number;
  estimatedValue!: number;
  status!: string; // InventoryItemStatus
  name!: string;
}
