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

  // 🆕 Новые поля для ТТН и партий
  batchNumber?: string;
  measureUnit?: string;
  vatRate?: number;
  placesCount?: number;
  cargoWeight?: number;
  notes?: string;
}
