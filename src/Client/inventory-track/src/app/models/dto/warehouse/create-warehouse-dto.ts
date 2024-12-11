import { WarehouseType } from './enums/warehouse-type.enum';

export class CreateWarehouseDto {
  name!: string;
  type!: WarehouseType;
  location!: string;
  companyId!: string;
  responsiblePersonId!: string;
}
