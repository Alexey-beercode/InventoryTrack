export interface GenerateInventoryDocumentDto {
  isWriteOff: boolean;
  quantity: number;
  sourceWarehouseId: string;       // для списания и перемещения
  destinationWarehouseId?: string; // только для перемещения
  inventoryItemId: string;
}
