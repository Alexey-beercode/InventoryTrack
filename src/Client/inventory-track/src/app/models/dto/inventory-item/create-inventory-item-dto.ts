export class CreateInventoryItemDto {
  name!: string;
  uniqueCode!: string;
  quantity!: number;
  estimatedValue!: number;
  expirationDate!: Date;
  supplierId!: string;
  warehouseId!: string;
  deliveryDate!: Date;
  documentFile!: File;
}
