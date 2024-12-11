export class CreateMovementRequestDto {
  itemId!: string;
  sourceWarehouseId!: string;
  destinationWarehouseId!: string;
  quantity!: number;
}
