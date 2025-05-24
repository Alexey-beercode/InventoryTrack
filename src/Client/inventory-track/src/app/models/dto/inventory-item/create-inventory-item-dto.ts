export class CreateInventoryItemDto {
  name!: string;
  uniqueCode!: string;
  quantity!: number;
  estimatedValue!: number;
  expirationDate!: string;
  supplierId!: string;
  warehouseId!: string;
  deliveryDate!: string;
  documentId!: string;

  // 🆕 Новые поля для ТТН
  batchNumber?: string;
  measureUnit?: string;
  vatRate?: number;
  placesCount?: number;
  cargoWeight?: number;
  notes?: string;
}
