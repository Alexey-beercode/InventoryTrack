import { WarehouseType } from './enums/warehouse-type.enum';

export class WarehouseTypeResponseDto {
  value!: WarehouseType; // Используем enum для value
  name!: string;
}
