import { WarehouseType } from './enums/warehouse-type.enum';

export class WarehouseResponseDto {
  id!: string;
  name!: string;
  type!: WarehouseType;
  location!: string;
  companyId!: string;
  responsiblePersonId!: string;
}
