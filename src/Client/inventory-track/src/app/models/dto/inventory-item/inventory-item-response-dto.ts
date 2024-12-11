// src/app/models/dto/inventory-item/inventory-item-response-dto.ts
import { SupplierResponseDto } from '../supplier/supplier-response-dto';
import { InventoryItemStatusResponseDto } from './inventory-item-status-response-dto';
import { WarehouseDetailsDto } from '../warehouse/warehouse-details-dto';
import { DocumentInfoResponseDto } from '../document/document-info-response-dto';

export class InventoryItemResponseDto {
  id!: string;
  name!: string;
  uniqueCode!: string;
  quantity!: number;
  estimatedValue!: number;
  expirationDate!: Date;
  supplier!: SupplierResponseDto;
  deliveryDate!: Date;
  documentId!: string;
  status!: InventoryItemStatusResponseDto;
  warehouseDetails!: WarehouseDetailsDto[];
  documentInfo!: DocumentInfoResponseDto;
}
