export class FilterInventoryItemDto {
  name?: string;
  supplierId?: string;
  expirationDateFrom?: Date;
  expirationDateTo?: Date;
  estimatedValue?: number;
}
