import { WarehouseTypeResponseDto } from './warehouse-type-response-dto';
import { InventoryItemResponseDto } from '../inventory-item/inventory-item-response-dto';

export class WarehouseStateResponseDto {
  id!: string;
  name!: string;
  type!: WarehouseTypeResponseDto;
  itemsCount!: number;
  location!: string;
  quantity!: number;
  inventoryItems!: InventoryItemResponseDto[];
}
