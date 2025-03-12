export class CreateInventoryItemDto {
  name!: string;
  uniqueCode!: string;
  quantity!: number;
  estimatedValue!: number;
  expirationDate!: string;  // 🔥 Изменено на string
  supplierId!: string;
  warehouseId!: string;
  deliveryDate!: string;  // 🔥 Изменено на string
  documentId!: string;
}
