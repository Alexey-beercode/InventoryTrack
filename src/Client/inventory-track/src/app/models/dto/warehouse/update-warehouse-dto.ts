// src/app/models/dto/warehouse/update-warehouse-dto.ts
import {WarehouseType} from "./enums/warehouse-type.enum";

export interface UpdateWarehouseDto {
  id: string;
  name: string;
  type: number; // WarehouseType в строковом формате
  location: string;
  companyId: string;
  responsiblePersonId: string;
}
