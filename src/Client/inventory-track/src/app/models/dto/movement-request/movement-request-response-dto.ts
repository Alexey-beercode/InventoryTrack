import { MovementRequestStatusResponseDto } from './movement-request-status-response-dto';

export class MovementRequestResponseDto {
  id!: string; // Assuming BaseDto has an id property
  itemId!: string;
  sourceWarehouseId!: string;
  destinationWarehouseId!: string;
  quantity!: number;
  status!: MovementRequestStatusResponseDto;
  requestDate!: Date;
  approvedByUserId?: string;
}
